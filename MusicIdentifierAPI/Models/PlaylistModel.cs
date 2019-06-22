namespace MusicIdentifierAPI.Models
{
    public class PlaylistModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public bool Public { get; set; }
        public string ShareLink { get; set; }
    }

    public class SongInPlaylist
    {
        public int PlaylistId { get; set; }
        public int SongId { get; set; }
    }
}