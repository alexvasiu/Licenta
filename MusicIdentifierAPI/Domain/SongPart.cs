using System.Collections.Generic;

namespace MusicIdentifierAPI.Domain
{
    public class SongPart
    {
        public int Id { get; set; }
        public string Hashtag { get; set; }
        public double Time { get; set; }
        public Song Song { get; set; }
        public int SongId { get; set; }
        public double Duration { get; set; }
        public List<double> HighScores { get; set; }
        public List<int> Points { get; set; }
    };
}