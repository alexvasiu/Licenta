using MusicIdentifierAPI.Domain;
using MusicIdentifierAPI.Models;

namespace MusicIdentifierAPI.Mappers
{
    public class UserMapper
    {
        public static UserModel MapUser(User user) => new UserModel
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            RefreshToken = user.RefreshToken,
            FacebookId = user.FacebookId,
            GoogleId = user.GoogleId,
            UserType = user.UserType
        };

        public static User MapUserModel(UserModel user) => new User
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            RefreshToken = user.RefreshToken,
            FacebookId = user.FacebookId,
            GoogleId = user.GoogleId,
            UserType = user.UserType
        };

        public static User MapRegisterModel(UserRegisterModel userRegisterModel) => new User
        {
            Username = userRegisterModel.Username,
            Email = userRegisterModel.Email,
            Password = userRegisterModel.Password,
            FacebookId = userRegisterModel.FacebookId,
            GoogleId = userRegisterModel.GoogleId,
            UserType = UserType.Normal
        };
    }
}