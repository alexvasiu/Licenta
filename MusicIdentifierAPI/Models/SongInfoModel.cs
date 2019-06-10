using System;

namespace MusicIdentifierAPI.Models
{
    public class SongInfoModel
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
        public byte[] Picture { get; set; }
    }
}