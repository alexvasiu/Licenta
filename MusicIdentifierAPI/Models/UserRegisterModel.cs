﻿namespace MusicIdentifierAPI.Models
{
    public class UserRegisterModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FacebookId { get; set; }
        public string GoogleId { get; set; }
    }
}