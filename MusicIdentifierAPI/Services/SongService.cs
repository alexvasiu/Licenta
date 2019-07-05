using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ANN;
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
        List<PlaylistModel> GetPlaylists(int songId, int userId);
        List<(string, double)> ClasifySong(byte[] data, string rootPath);
        List<SongInfoModel> GetSongsFromPlaylist(int playlistId);
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

        public PlaylistModel AddPlaylist(PlaylistModel playlistModel)
        {
            using var unitOfWork = new UnitOfWork();
            var playlistRepo = unitOfWork.GetRepository<Playlist>();
            var playlist = PlaylistMapper.MapPlaylist(playlistModel);
            playlistRepo.Add(playlist);

            return PlaylistMapper.MapPlaylist(playlist);
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

        public List<PlaylistModel> GetPlaylists(int songId, int userId)
        {
            using var unitOfWork = new UnitOfWork();
            var playlistRepo = unitOfWork.GetRepository<Playlist>();
            var playlistSongRepo = unitOfWork.GetRepository<SongPlaylist>();

            var founded = playlistSongRepo.GetAll().Where(x => x.SongId == songId).Select(x => x.PlaylistId);

            return playlistRepo.GetAll().Where(x => x.UserId == userId && !founded.Contains(x.Id)).Select(x => PlaylistMapper.MapPlaylist(x)).ToList();
        }

        public List<(string, double)> ClasifySong(byte[] data, string rootPath)
        {
            var netFilePath = Path.Combine(rootPath, "net");

            var ann = new RNA(null, AnnMode.Training, ActivationFunctions.Tahn);
            var net = ANN.Utils.ReadFromBinaryFile<List<List<Neuron>>>(netFilePath);

            var sound2 = SoundReader.ReadFromData(data, SoundType.Wav);
            var result2 = Fft.CalculateFft(sound2, keepAll: true);

            ann.TestData = result2.Result.Select(x => (x.HighScores, "x")).ToList();
            ann.TestData.Shuffle();

            var computedOutputs = ann.Evaluate(ref net, 10);

            var genres = ann.GetGeneres();
            foreach (var result in computedOutputs)
                genres[ann.GetValueFromLabelVector(result)] += 1;

            var res = new List<(string, double)>();
            foreach (var (key, value) in genres)
                res.Add((key, value * 100.0 / computedOutputs.Count));

            return res;
        }

        public List<SongInfoModel> GetSongsFromPlaylist(int playlistId)
        {
            using var unitOfWork = new UnitOfWork();
            var songRepo = unitOfWork.GetRepository<Song>();
            var songPlaylistRepo = unitOfWork.GetRepository<SongPlaylist>();

            return songPlaylistRepo.GetAll().Where(x => x.PlaylistId == playlistId)
                .Select(x => SongMapper.MapSong(songRepo.Find(x.SongId))).ToList();

        }
    }
}