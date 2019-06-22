using MusicIdentifierAPI.Domain;

namespace MusicIdentifierAPI.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string FacebookId { get; set; }
        public string GoogleId { get; set; }
        public UserType UserType { get; set; }
    }

    public class UserChangePassword
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserChangeProfile
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FacebookId { get; set; }
        public string GoogleId { get; set; }
        public UserType UserType { get; set; }
    }
}