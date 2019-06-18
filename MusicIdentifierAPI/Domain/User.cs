using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MusicIdentifierAPI.Domain
{
    [Serializable]
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string RefreshToken { get; set; }
        [DefaultValue(null)]
        public string FacebookId { get; set; }
        [DefaultValue(null)]
        public string GoogleId { get; set; }
        public List<Playlist> Playlists { get; set; }
        [DefaultValue(UserType.Normal)]
        public UserType UserType { get; set; }
        public User()
        {
            Playlists = new List<Playlist>();
        }
    }

    public enum UserType
    {
        Normal,
        Admin
    }
}