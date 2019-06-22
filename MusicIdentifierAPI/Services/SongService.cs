using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Hangfire;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
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
        SongInfoModel AnalyzeAudioFile(byte[] data, string rootPath);
        PlaylistModel AddPlaylist(PlaylistModel playlistModel);
        bool AddSongToPlaylist(SongInPlaylist songInPlaylist);
    }
    public class SongService : ISongService
    {
        private readonly AppSettings _appSettings;

        public SongService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public SongInfoModel AnalyzeAudioFile(byte[] data, string rootPath)
        {
            using var unitOfWork = new UnitOfWork();
            var songRepo = unitOfWork.GetRepository<Song>();
           // var songs = songRepo.GetAll().ToList();

            var dictFilePath = Path.Combine(rootPath, "songDict");

            var dict =
                Utils.Utils.ReadFromBinaryFile<SortedDictionary<string, SortedDictionary<int, SongPart>>>(dictFilePath);

            if (dict == null)
                throw new Exception("Dictionary file not found");

            /*if (dict == null)
            {
                var songPartRepo = unitOfWork.GetRepository<SongPart>();
                var dict = new SortedDictionary<string, SortedDictionary<int, SongPart>>();
                foreach (var songPart in songPartRepo.GetAll())
                {
                    if (songPart.Hashtag == "0") continue;
                    if (dict.ContainsKey(songPart.Hashtag))
                        dict[songPart.Hashtag].Add(songPart.Id, songPart);
                    else
                        dict[songPart.Hashtag] = new SortedDictionary<int, SongPart> { { songPart.Id, songPart } };
                }
                // Utils.Utils.WriteToBinaryFile("songDict", dict);
            //}*/

            var sound = SoundReader.ReadFromData(data, SoundType.Wav);
            var resultFft = Fft.CalculateFft(sound, false);

           /* var correct = new SortedDictionary<string, DataPoint>();
            foreach (var t in resultFft.Result)
                if (dict.ContainsKey(t.Hash) && !correct.ContainsKey(t.Hash))
                    correct.Add(t.Hash, t);*/


            for (var i1 = 0; i1 < resultFft.Result.Count; i1++)
                for (var i2 = i1 + 1; i2 < resultFft.Result.Count; i2++)
            {
                var hash1 = resultFft.Result[i1].Hash;
                var hash2 = resultFft.Result[i2].Hash;
                if (!dict.ContainsKey(hash1) || !dict.ContainsKey(hash2)) continue;
                var all1 = dict[hash1];
                foreach (var (_, value) in all1)
                {
                    if (!dict[hash2].Any(x => value.SongId == x.Value.SongId &&
                                              Math.Abs(Math.Abs(resultFft.Result[i1].Time -
                                                                resultFft.Result[i2].Time) -
                                                       Math.Abs(value.Time - x.Value.Time)) <
                                              0.0001)) continue;
                    var song = songRepo.Find(value.SongId);
                    song.IdentificationCounter += 1;
                    songRepo.Update(song);
                    unitOfWork.Save();
                    return SongMapper.MapSong(song);
                }
            }

            return null;
        }

        public PlaylistModel AddPlaylist(PlaylistModel playlistModel)
        {
            using var unitOfWork = new UnitOfWork();
            var playlistRepo = unitOfWork.GetRepository<Playlist>();
            var playlist = new Playlist
            {
                UserId = playlistModel.UserId,
                Name = playlistModel.Name,
                Public = playlistModel.Public,
                ShareLink = playlistModel.ShareLink
            };
            playlistRepo.Add(playlist);

            return new PlaylistModel
            {
                UserId = playlistModel.UserId,
                Name = playlistModel.Name,
                Public = playlistModel.Public,
                ShareLink = playlistModel.ShareLink,
                Id = playlistModel.Id
            };
        }

        public bool AddSongToPlaylist(SongInPlaylist songInPlaylist)
        {
            using var unitOfWork = new UnitOfWork();
            var playlistSongRepo = unitOfWork.GetRepository<SongPlaylist>();

            playlistSongRepo.Add(new SongPlaylist
            {
                PlaylistId = songInPlaylist.PlaylistId,
                SongId = songInPlaylist.SongId
            });

            unitOfWork.Save();
            return true;
        }
    }
}