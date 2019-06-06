namespace MusicIdentifierAPI.Domain
{
    public class SongPart
    {
        public int Id { get; set; }
        public string Hashtag { get; set; }
        public double Time { get; set; }
        public Song Song { get; set; }
        public int SongId { get; set; }
    };
}