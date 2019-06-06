namespace MusicIdentifierAPI.Domain
{
    public class Playlist
    {
        public int Id { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public Song Song { get; set; }
        public int SongId { get; set; }
        public string Name { get; set; }
        public bool Public { get; set; }
        public string ShareLink { get; set; }
    }
}