using System.Linq;
using MusicIdentifierAPI.Domain;
using MusicIdentifierAPI.Models;

namespace MusicIdentifierAPI.Mappers
{
    public class SongMapper
    {
        public static SongInfoModel MapSong(Song song) => new SongInfoModel
        {
            Id = song.Id,
            ApparitionDate = song.ApparitionDate,
            Artist = song.Artist,
            Duration = song.Duration,
            Genre = song.Genre,
            IdentificationCounter = song.IdentificationCounter,
            Name = song.Name,
            Picture = song.Picture?.ToArray(),
            SpotifyLink = song.SpotifyLink,
            YoutubeLink = song.YoutubeLink,
            BeatPortLink = song.BeatPortLink
        };
    }
}