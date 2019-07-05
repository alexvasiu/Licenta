using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ANN;
using Music_Extract_Feature;
using MusicIdentifierAPI.Domain;
using MusicIdentifierAPI.Repository;
using Newtonsoft.Json;
using Path = System.IO.Path;

namespace PopulateDatabase
{
    internal class Program
    {
        private static void DeleteAllData()
        {
            using var unitOfWork = new UnitOfWork();
            var userRepo = unitOfWork.GetRepository<User>();
            var playlistRepo = unitOfWork.GetRepository<Playlist>();
            var songRepo = unitOfWork.GetRepository<Song>();
            var songPartsRepo = unitOfWork.GetRepository<SongPart>();

            foreach (var playlist in playlistRepo.GetAll())
                playlistRepo.Remove(playlist.Id);

            foreach (var songPart in songPartsRepo.GetAll())
                playlistRepo.Remove(songPart.Id);

            foreach (var song in songRepo.GetAll())
                playlistRepo.Remove(song.Id);

            foreach (var user in userRepo.GetAll())
                playlistRepo.Remove(user.Id);

            unitOfWork.Save();
        }

        private static void AddSongToDb(string musicPath, MusicItem musicItem)
        {
            using var unitOfWork = new UnitOfWork();
            var songRepo = unitOfWork.GetRepository<Song>();
            var songPartsRepo = unitOfWork.GetRepository<SongPart>();

            var sound = SoundReader.ReadFromFile(Path.Combine(musicPath, musicItem.File));

            var newSong = new Song
            {
                Name = musicItem.Name,
                ApparitionDate = musicItem.ApparitionDate,
                Artist = musicItem.Artist,
                BeatPortLink = musicItem.BeatportLink,
                SpotifyLink = musicItem.SpotifyLink,
                YoutubeLink = musicItem.YoutubeLink,
                Genre = musicItem.Genre,
                Picture = musicItem.PictureFile == "" ? null : 
                    File.ReadAllBytes(Path.Combine(musicPath, musicItem.PictureFile)).ToArray(),
                Duration = sound.Duration
            };
            songRepo.Add(newSong);

            var resultFft = Fft.CalculateFft(sound);
            resultFft.Result.ForEach(x => songPartsRepo.Add(new SongPart
                {
                    SongId = newSong.Id,
                    Hashtag = x.Hash.ToString(),
                    Time = x.Time,
                    Duration = x.Duration,
                    HighScores = x.HighScores,
                    Points = x.Points
                })
            );
            unitOfWork.Save();
            unitOfWork.Dispose();
        }

        private static void GenerateJson(string musicPath)
        {
            var list = new List<MusicItem>();
            var genres = new List<string> { "blues", "classical", "country", "disco", "hiphop", "jazz", "metal", "pop", "reggae", "rock" };
            foreach (var genre in genres)
            {
                for (var i = 0; i < /*100*/ 2; i++)
                {
                    var filename = $@"genres\{genre}\{genre}.000{(i < 10 ? "0" + i: i.ToString())}.wav";
                    list.Add(new MusicItem
                    {
                        ApparitionDate = DateTime.Now.AddYears(-10),
                        Artist = "Unknown",
                        BeatportLink = "",
                        File = filename,
                        Genre = genre,
                        Name = $"{genre}.{i}",
                        PictureFile = "",
                        SpotifyLink = "",
                        YoutubeLink = ""
                    });
                }
            }

            using var file = File.CreateText(Path.Combine(musicPath, "newMusic.json"));
            var serializer = new JsonSerializer {Formatting = Formatting.Indented};
            serializer.Serialize(file, list);
        }   
        
        private static void Main(string[] args)
        {
            var executableLocation = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);
            var musicPath = Path.GetFullPath(Path.Combine(executableLocation, @"..\..\..\music"));
            // GenerateJson(musicPath);
            using var red = new StreamReader(Path.Combine(musicPath, "music.json"));
            var musicItems = JsonConvert.DeserializeObject<List<MusicItem>>(red.ReadToEnd());
            var percent = 0.0;
            Console.WriteLine($"Progress: {percent}%");
            foreach (var musicItem in musicItems)
            {
                Console.WriteLine($"{musicItem.File} in progress");
                AddSongToDb(musicPath, musicItem);
                percent += 100.0 / musicItems.Count;
                Console.Clear();
                Console.WriteLine($"Progress: {percent}%");
            }

            /*Console.WriteLine("Create RNA File ...");
            using var unitOfWork = new UnitOfWork();
            var songRepo = unitOfWork.GetRepository<Song>();
            var songPartsRepo = unitOfWork.GetRepository<SongPart>();

            var data = new List<(List<double>, string)>();
            foreach (var songPart in songPartsRepo.GetAll())
            {
                var song = songRepo.Find(songPart.SongId);
                data.Add((songPart.HighScores, song.Genre));
            }

            var ann = new RNA(data, AnnMode.Training, ActivationFunctions.Tahn);
            var net = ann.NetInit(10, 9);
            ann.Training(ref net, 10, 0.001, 1000);

            //MusicIdentifierAPI.Utils.Utils.WriteToBinaryFile("ann", ann);
            MusicIdentifierAPI.Utils.Utils.WriteToBinaryFile("net", net);*/

            Console.WriteLine("Done :)");
            Console.ReadKey();
        }
    }

    internal class MusicItem
    {
        public string Name { get; set; }
        public string Artist { get; set; }
        public string Genre { get; set; }
        public string File { get; set; }
        public DateTime ApparitionDate { get; set; }
        public string YoutubeLink { get; set; }
        public string SpotifyLink { get; set; }
        public string BeatportLink { get; set; }
        public string PictureFile { get; set; }
    }
}
