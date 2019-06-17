using System;
using System.Collections.Generic;

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
        public List<byte> Picture { get; set; }
        public List<SongPart> SongParts { get; set; }
        public List<SongPlaylist> Playlists { get; set; }

        public Song()
        {
            SongParts = new List<SongPart>();
            Picture = new List<byte>();
            Playlists = new List<SongPlaylist>();
        }
    }
}