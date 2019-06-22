namespace MusicIdentifierAPI.Models
{
    public class UserLoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserLoginFacebook
    {
        public string FacebookId { get; set; }
        public string Email { get; set; }
    }

    public class UserLoginGoogle
    {
        public string GoogleId { get; set; }
        public string Email { get; set; }
    }
}