using Lingua.ZoomIntegration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Lingua.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _zoomAuthService;
        private readonly IUserService _zoomUserService;
        private readonly Shared.Users.IUserService _userService;

        public AuthController(IAuthService zoomAuthService, IUserService zoomUserService, Shared.Users.IUserService userService)
        {
            _zoomAuthService = zoomAuthService;
            _zoomUserService = zoomUserService;
            _userService = userService;
        }

        [HttpGet]
        [Route("login/zoom")]
        public async Task<IActionResult> ZoomLogin(string authCode)
        {
            var response = await _zoomAuthService.RequestAccessToken(authCode);
            var zoomUser = await _zoomUserService.GetUserProfile(response.AccessToken);
            var user = (await _userService.Get(u => u.Email == zoomUser.Email)).FirstOrDefault();
            if (user == null)
            {
                user = new Shared.User()
                {
                    Email = zoomUser.Email,
                    Firstname = zoomUser.Firstname,
                    Lastname = zoomUser.Lastname,
                    ZoomProperties = new Shared.ZoomProperties
                    {
                        AccessToken = response.AccessToken,
                        RefreshToken = response.RefreshToken
                    }
                };
                await _userService.Create(user);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties { IsPersistent = true });

            return Ok();
        }
    }
}
