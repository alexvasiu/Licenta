using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicIdentifierAPI.Models;
using MusicIdentifierAPI.Services;

namespace MusicIdentifierAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [AllowAnonymous]
        [Produces("application/json")]
        [Route("login")]
        public IActionResult LoginIn([FromBody] UserLoginModel userLoginModel)
        {
            var loginResult = _userService.Login(userLoginModel);
            return loginResult == null ? Unauthorized() as IActionResult : new OkObjectResult(loginResult);
        }

        [HttpPost]
        [AllowAnonymous]
        [Produces("application/json")]
        [Route("loginFacebook")]
        public IActionResult LoginInFb([FromBody] UserLoginFacebook userLoginFacebook)
        {
            var loginResult = _userService.LoginFb(userLoginFacebook);
            return loginResult == null ? Unauthorized() as IActionResult : new OkObjectResult(loginResult);
        }

        [HttpPost]
        [AllowAnonymous]
        [Produces("application/json")]
        [Route("loginGoogle")]
        public IActionResult LoginInGoogle([FromBody] UserLoginGoogle userLoginGoogle)
        {
            var loginResult = _userService.LoginGoogle(userLoginGoogle);
            return loginResult == null ? Unauthorized() as IActionResult : new OkObjectResult(loginResult);
        }

        [HttpPost]
        [Produces("application/json")]
        [Route("changePassword")]
        public IActionResult ChangePassword([FromBody] UserChangePassword userChangePassword)
        {
            var loginResult = _userService.ChangePassword(userChangePassword);
            return !loginResult ? Unauthorized() as IActionResult : new OkObjectResult(true);
        }

        [HttpPost]
        [Produces("application/json")]
        [Route("changeProfile")]
        public IActionResult ChangeProfile([FromBody] UserChangeProfile userChangeProfile)
        {
            var loginResult = _userService.ChangeProfile(userChangeProfile);
            return !loginResult ? Unauthorized() as IActionResult : new OkObjectResult(true);
        }

        [HttpPost]
        [AllowAnonymous]
        [Produces("application/json")]
        [Route("refreshToken")]
        public IActionResult Refresh([FromBody] RefreshTokenModel refreshTokenModel)
        {
            return new OkObjectResult(_userService.Refresh(refreshTokenModel.Token, refreshTokenModel.RefreshToken));
        }

        [HttpPost]
        [AllowAnonymous]
        [Produces("application/json")]
        [Route("register")]
        public IActionResult Register([FromBody] UserRegisterModel model)
        {
            try
            {
                return new OkObjectResult(_userService.Register(model));
            }
            catch (Exception exception)
            {
                return new BadRequestObjectResult(exception.Message);
            }
        }
    }
}