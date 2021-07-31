using Lingua.Shared;
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
        private readonly IUserClient _zoomUserService;
        private readonly IUserRepository _userRepository;

        public AuthController(IAuthClient zoomAuthService, IUserClient zoomUserService, IUserRepository userRepository)
        {
            _zoomAuthService = zoomAuthService;
            _zoomUserService = zoomUserService;
            _userRepository = userRepository;
        }

        [HttpGet]
        [Route("login/zoom")]
        public async Task<IActionResult> ZoomLogin(string authCode)
        {
            var tokens = await _zoomAuthService.RequestAccessToken(authCode);
            var response = await _zoomUserService.GetUserProfile(tokens);
            var zoomUser = response.Response;
            
            var user = (await _userRepository.Get(u => u.Email == zoomUser.Email)).FirstOrDefault();
            var isNewAccount = user == null;
            if (isNewAccount)
            {
                user = new User()
                {
                    Email = zoomUser.Email,
                    Firstname = zoomUser.Firstname,
                    Lastname = zoomUser.Lastname,
                    AvatarUrl = zoomUser.PicUrl,
                    ZoomProperties = new ZoomProperties
                    {
                        AccessTokens = tokens
                    }
                };
                await _userRepository.Create(user);
            }
            else
            {
                if (user.AvatarUrl == null)
                {
                    user.AvatarUrl = zoomUser.PicUrl;
                }
                user.ZoomProperties.AccessTokens = tokens;
                await _userRepository.Update(user);
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
                var user = await _userRepository.Get(userId);
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
                var user = await _userRepository.Get(userId);

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            return Ok();
        }
    }
}
