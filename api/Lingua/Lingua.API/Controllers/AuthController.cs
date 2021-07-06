using Lingua.ZoomIntegration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
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
        private readonly IAuthClient _zoomAuthService;
        private readonly IUserService _zoomUserService;
        private readonly Shared.Users.IUserService _userService;

        public AuthController(IAuthClient zoomAuthService, IUserService zoomUserService, Shared.Users.IUserService userService)
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
            var isNewAccount = user == null;
            if (isNewAccount)
            {
                user = new Shared.User()
                {
                    Email = zoomUser.Email,
                    Firstname = zoomUser.Firstname,
                    Lastname = zoomUser.Lastname,
                    ZoomProperties = new Shared.ZoomProperties
                    {
                        AccessTokens = response
                    }
                };
                await _userService.Create(user);
            }
            else
            {
                user.ZoomProperties.AccessTokens = response;
                await _userService.Update(user);
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

            return Ok(new { isNewAccount });
        }

        [HttpGet]
        [Route("login")]
        public async Task<IActionResult> Login()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                var user = await _userService.Get(userId);
                return Ok();
            }

            return StatusCode(403);
        }

        [HttpGet]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                var user = await _userService.Get(userId);

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            return Ok();
        }
    }
}
