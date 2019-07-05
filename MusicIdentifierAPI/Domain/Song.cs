using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using EntityFrameworkCore.Triggers;

namespace MusicIdentifierAPI.Domain
{
    [Serializable]
    public class Song
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Artist { get; set; }
        public string Genre { get; set; }
        public DateTime ApparitionDate { get; set; }
        public int IdentificationCounter { get; set; }
        public string YoutubeLink { get; set; }
        public string SpotifyLink { get; set; }
        public string BeatPortLink { get; set; }
        public double Duration { get; set; }
        [Column(TypeName = "bytea")]
        public Byte[] Picture { get; set; }
        public List<SongPart> SongParts { get; set; }
        public List<SongPlaylist> Playlists { get; set; }

        public Song()
        {
            SongParts = new List<SongPart>();
            Playlists = new List<SongPlaylist>();
        }

        static Song()
        {
            Triggers<Song>.Inserted += songObj =>
            {
                var song = songObj.Entity;
                if (song.SongParts == null) return;
                var currentDirectory = Directory.GetCurrentDirectory();
                var finalPath =
                    currentDirectory.Contains("MusicIdentifierAPI")
                        ? "songDict"
                        : Path.GetFullPath(Path.Combine(currentDirectory, @"..\..\..\..\MusicIdentifierAPI\wwwroot\songDict"));
                var dict =
                    Utils.Utils.ReadFromBinaryFile<SortedDictionary<string, SortedDictionary<int, SongPart>>>(finalPath) ??
                    new SortedDictionary<string, SortedDictionary<int, SongPart>>();
                foreach (var songPart in song.SongParts)
                {
                    if (songPart.Hashtag == "0") continue;
                    if (dict.ContainsKey(songPart.Hashtag))
                        dict[songPart.Hashtag].Add(songPart.Id, songPart);
                    else
                        dict[songPart.Hashtag] = new SortedDictionary<int, SongPart> { { songPart.Id, songPart } };
                }
                Utils.Utils.WriteToBinaryFile(finalPath, dict);
            };
        }
    }
}