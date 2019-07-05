using MusicIdentifierAPI.Domain;
using MusicIdentifierAPI.Models;

namespace MusicIdentifierAPI.Mappers
{
    public class PlaylistMapper
    {
        public static PlaylistModel MapPlaylist(Playlist playlist) => new PlaylistModel
        {
            Id = playlist.Id,
            Name = playlist.Name,
            Public = playlist.Public,
            UserId = playlist.UserId,
            ShareLink = playlist.ShareLink
        };

        public static Playlist MapPlaylist(PlaylistModel playlistModel) => new Playlist
        {
            Id = playlistModel.Id,
            Name = playlistModel.Name,
            Public = playlistModel.Public,
            UserId = playlistModel.UserId,
            ShareLink = playlistModel.ShareLink
        };
    }
}
