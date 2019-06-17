using System;
using System.Collections.Generic;
using EntityFrameworkCore.Triggers;

namespace MusicIdentifierAPI.Domain
{
    [Serializable]
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

        static SongPart()
        {
            Triggers<SongPart>.Inserted += songPartObj =>
            {
                var dict =
                    Utils.Utils.ReadFromBinaryFile<SortedDictionary<string, SortedList<double, SongPart>>>("songDict");
                var songPart = songPartObj.Entity;
                if (dict.ContainsKey(songPart.Hashtag))
                    dict[songPart.Hashtag].Add(songPart.Time, songPart);
                else
                    dict[songPart.Hashtag] = new SortedList<double, SongPart> { { songPart.Time, songPart } };
                Utils.Utils.WriteToBinaryFile("songDict", dict);
            };
        }
    };
}