using System;
using System.Collections.Generic;
using System.Linq;
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
            var songPartRepo = unitOfWork.GetRepository<SongPart>();

            var allParts = songPartRepo.GetAll();

            var dict = new SortedDictionary<string, SortedList<double, SongPart>>();
            foreach (var songPart in allParts)
            {
                if (dict.ContainsKey(songPart.Hashtag))
                    dict[songPart.Hashtag].Add(songPart.Time, songPart);
                else
                    dict[songPart.Hashtag] = new SortedList<double, SongPart> {{songPart.Time, songPart}};
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
                    foreach (var (key, value) in all1)
                    {
                        if (dict[hash2].Any(x => value.SongId == x.Value.SongId &&
                                                 Math.Abs(Math.Abs(resultFft.Result[i1].Time -
                                                                   resultFft.Result[i2].Time) - Math.Abs(key - x.Key)) < 0.001))
                            return SongMapper.MapSong(songRepo.Find(value.SongId));
                    }
                }

            return null;

            #region oldVersionOfCode
            /*for (var i1 = 0; i1 < resultFft.Result.Count; i1++)
               for (var i2 = i1 + 1; i2 < resultFft.Result.Count; i2++)
               {
                   // var all1 = allParts.Where(x => x.Points == resultFft.Result[i1].Points).ToList();
                   var all1 = allParts.Where(x => x.Hashtag == resultFft.Result[i1].Hash.ToString()).ToList();

                   foreach (var res in all1)
                   {
                       if (allParts.Any(x => res.SongId == x.SongId &&
                                             Math.Abs(Math.Abs(resultFft.Result[i1].Time -
                                                               resultFft.Result[i2].Time) - Math.Abs(res.Time - x.Time)) < 0.001 
                                             && x.Points == resultFft.Result[i2].Points))
                       if (allParts.Any(x => res.SongId == x.SongId &&
                                             Math.Abs(Math.Abs(resultFft.Result[i1].Time -
                                                               resultFft.Result[i2].Time) - Math.Abs(res.Time - x.Time)) < 0.001
                                             && x.Hashtag == resultFft.Result[i2].Hash.ToString()))
                           return SongMapper.MapSong(songRepo.Find(res.SongId));
                   }
               }*/
            #endregion

        }
    }
}