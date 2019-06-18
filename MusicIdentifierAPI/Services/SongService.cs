using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Hangfire;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Music_Extract_Feature;
using MusicIdentifierAPI.Domain;
using MusicIdentifierAPI.Mappers;
using MusicIdentifierAPI.Models;
using MusicIdentifierAPI.Repository;

namespace MusicIdentifierAPI.Services
{
    public interface ISongService
    {
        SongInfoModel AnalyzeAudioFile(byte[] data);
    }
    public class SongService : ISongService
    {
        private readonly AppSettings _appSettings;

        public SongService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public SongInfoModel AnalyzeAudioFile(byte[] data)
        {
            using var unitOfWork = new UnitOfWork();
            var songRepo = unitOfWork.GetRepository<Song>();

            var dict =
                Utils.Utils.ReadFromBinaryFile<SortedDictionary<string, SortedDictionary<int, SongPart>>>("songDict");

            if (dict == null)
            {
                var songPartRepo = unitOfWork.GetRepository<SongPart>();
                dict = new SortedDictionary<string, SortedDictionary<int, SongPart>>();
                foreach (var songPart in songPartRepo.GetAll())
                {
                    if (songPart.Hashtag == "0") continue;
                    if (dict.ContainsKey(songPart.Hashtag))
                        dict[songPart.Hashtag].Add(songPart.Id, songPart);
                    else
                        dict[songPart.Hashtag] = new SortedDictionary<int, SongPart> { { songPart.Id, songPart } };
                }
                Utils.Utils.WriteToBinaryFile("songDict", dict);
            }

            var sound = SoundReader.ReadFromData(data, SoundType.Wav);
            var resultFft = Fft.CalculateFft(sound);

            for (var i1 = 0; i1 < resultFft.Result.Count; i1++)
                for (var i2 = i1 + 1; i2 < resultFft.Result.Count; i2++)
                {
                    var hash1 = resultFft.Result[i1].Hash.ToString();
                    var hash2 = resultFft.Result[i2].Hash.ToString();
                    if (!dict.ContainsKey(hash1) || !dict.ContainsKey(hash2)) continue;
                    var all1 = dict[hash1];
                    foreach (var (_, value) in all1)
                    {
                        if (!dict[hash2].Any(x => value.SongId == x.Value.SongId &&
                                                  Math.Abs(Math.Abs(resultFft.Result[i1].Time -
                                                                    resultFft.Result[i2].Time) -
                                                           Math.Abs(value.Time - x.Value.Time)) <
                                                  0.001)) continue;
                        var song = songRepo.Find(value.SongId);
                        song.IdentificationCounter += 1;
                        songRepo.Update(song);
                        unitOfWork.Save();
                        return SongMapper.MapSong(song);
                    }
                }

            return null;
        }
    }
}