﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MusicIdentifierAPI.Domain;
using MusicIdentifierAPI.Mappers;
using MusicIdentifierAPI.Models;
using MusicIdentifierAPI.Repository;
using MusicIdentifierAPI.Utils;

namespace MusicIdentifierAPI.Services
{
    public interface IUserService
    {
        UserModel Login(UserLoginModel userLoginModel);
        UserModel Refresh(string token, string refreshToken);
        UserModel Register(UserRegisterModel model);
        UserModel LoginFb(UserLoginFacebook userLoginFacebook);
        UserModel LoginGoogle(UserLoginGoogle userLoginGoogle);
        bool ChangePassword(UserChangePassword userChangePassword);
        bool ChangeProfile(UserChangeProfile userChangeProfile);
    }
    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public UserModel LoginFb(UserLoginFacebook userLoginFacebook)
        {
            using var unitOfWork = new UnitOfWork();
            var userRepo = unitOfWork.GetRepository<User>();
            var user = userRepo.FirstOrDefault(x => x.FacebookId == userLoginFacebook.FacebookId);
            if (user == null)
            {
                user = new User
                {
                    Email = userLoginFacebook.Email,
                    FacebookId = userLoginFacebook.FacebookId,
                    GoogleId = null,
                    UserType = UserType.Normal,
                    Username = userLoginFacebook.Email,
                    RefreshToken = GenerateRefreshToken(),
                    Password = StringCipher.Encrypt("", "KI6rnfCy6YUFq0mLoO")
                };
                userRepo.Add(user);
                var playlistRepo = unitOfWork.GetRepository<Playlist>();
                playlistRepo.Add(new Playlist
                {
                    Name = "Liked songs",
                    Public = false,
                    ShareLink = "",
                    UserId = user.Id
                });

                unitOfWork.Save();
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddMonths(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var userModel = UserMapper.MapUser(user);
            userModel.Token = tokenHandler.WriteToken(token);
            return userModel;
        }

        public UserModel LoginGoogle(UserLoginGoogle userLoginGoogle)
        {
            using var unitOfWork = new UnitOfWork();
            var userRepo = unitOfWork.GetRepository<User>();
            var user = userRepo.FirstOrDefault(x => x.GoogleId == userLoginGoogle.GoogleId);
            if (user == null)
            {
                user = new User
                {
                    Email = userLoginGoogle.Email,
                    GoogleId = userLoginGoogle.GoogleId,
                    FacebookId = null,
                    UserType = UserType.Normal,
                    Username = userLoginGoogle.Email,
                    RefreshToken = GenerateRefreshToken(),
                    Password = StringCipher.Encrypt("", "KI6rnfCy6YUFq0mLoO")
                };
                userRepo.Add(user);
                var playlistRepo = unitOfWork.GetRepository<Playlist>();
                playlistRepo.Add(new Playlist
                {
                    Name = "Liked songs",
                    Public = false,
                    ShareLink = "",
                    UserId = user.Id
                });

                unitOfWork.Save();
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddMonths(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var userModel = UserMapper.MapUser(user);
            userModel.Token = tokenHandler.WriteToken(token);
            return userModel;
        }

        public bool ChangePassword(UserChangePassword userChangePassword)
        {
            using var unitOfWork = new UnitOfWork();
            var userRepo = unitOfWork.GetRepository<User>();
            var user = userRepo.FirstOrDefault(x =>
                x.Username == userChangePassword.Username);
            if (user == null)
                return false;
            user.Password = StringCipher.Encrypt(userChangePassword.Password, "KI6rnfCy6YUFq0mLoO");
            userRepo.Update(user);
            unitOfWork.Save();
            return true;
        }

        public bool ChangeProfile(UserChangeProfile userChangeProfile)
        {
            using var unitOfWork = new UnitOfWork();
            var userRepo = unitOfWork.GetRepository<User>();
            var user = userRepo.FirstOrDefault(x =>
                x.Username == userChangeProfile.Username);
            if (user == null)
                return false;
            user.FacebookId = userChangeProfile.FacebookId;
            user.GoogleId = userChangeProfile.GoogleId;
            user.Email = userChangeProfile.Email;
            user.UserType = userChangeProfile.UserType;
            userRepo.Update(user);
            unitOfWork.Save();
            return true;
        }

        public UserModel Login(UserLoginModel userLoginModel)
        {
            using var unitOfWork = new UnitOfWork();
            var userRepo = unitOfWork.GetRepository<User>();
            userLoginModel.Password = StringCipher.Encrypt(userLoginModel.Password, "KI6rnfCy6YUFq0mLoO");
            var user = userRepo.FirstOrDefault(x =>
                x.Username == userLoginModel.Username && x.Password == userLoginModel.Password);
            if (user == null)
                return null;
            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddMonths(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var userModel = UserMapper.MapUser(user);
            userModel.Token = tokenHandler.WriteToken(token);
            return userModel;
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Secret)),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public UserModel Refresh(string token, string refreshToken)
        {
            using var unitOfWork = new UnitOfWork();
            var userRepo = unitOfWork.GetRepository<User>();
            var principal = GetPrincipalFromExpiredToken(token);
            var username = principal.Identity.Name;
            var user = userRepo.FirstOrDefault(x => x.Username == username);
            if (user.RefreshToken != refreshToken)
                throw new SecurityTokenException("Invalid refresh token");
            
            user.RefreshToken = GenerateRefreshToken();
            userRepo.Update(user);
            unitOfWork.Save();

            var userModel = UserMapper.MapUser(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var newToken = tokenHandler.CreateToken(tokenDescriptor);
            userModel.Token = tokenHandler.WriteToken(newToken);
            return userModel;
        }

        public UserModel Register(UserRegisterModel model)
        {
            using var unitOfWork = new UnitOfWork();
            var userRepo = unitOfWork.GetRepository<User>();
            var playlistRepo = unitOfWork.GetRepository<Playlist>();
            var user = UserMapper.MapRegisterModel(model);
            if (userRepo.FirstOrDefault(x => x.Username == user.Username) != null)
                throw new Exception("Username already exists");
            if (userRepo.FirstOrDefault(x => x.Email == user.Email) != null)
                throw new Exception("Email already exists");
            user.RefreshToken = GenerateRefreshToken();
            user.Password = StringCipher.Encrypt(user.Password, "KI6rnfCy6YUFq0mLoO");
            userRepo.Add(user);
            playlistRepo.Add(new Playlist
            {
                Name = "Liked songs",
                Public = false,
                ShareLink = "",
                UserId = user.Id
            });

            unitOfWork.Save();
            return UserMapper.MapUser(user);
        }
    }
}