using System;

namespace MusicIdentifierAPI.Domain
{
    [Serializable]
    public class SongPlaylist
    {
        public int Id { get; set; }
        public Playlist Playlist { set; get; }
        public int PlaylistId { get; set; }
        public Song Song { get; set; }
        public int SongId { get; set; }
    }
}