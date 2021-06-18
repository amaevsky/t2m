using Lingua.ZoomIntegration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Lingua.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthControler : ControllerBase
    {
        private readonly IAuthService _zoomAuthService;
        private readonly IUserService _zoomUserService;

        public AuthControler(IAuthService zoomAuthService, IUserService zoomUserService)
        {
            _zoomAuthService = zoomAuthService;
            _zoomUserService = zoomUserService;
        }

        [HttpGet]
        [Route("login/zoom")]
        public async Task<IActionResult> ZoomLogin(string authCode)
        {
            var response = await _zoomAuthService.RequestAccessToken(authCode);
            var user = await _zoomUserService.GetUserProfile(response.AccessToken);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("Firstname", user.Firstname),
                new Claim("Lastname", user.Lastname),
                new Claim("ZoomAccessToken", response.AccessToken),
                new Claim("ZoomRefreshToken", response.RefreshToken),
                new Claim("UserId", user.Id)
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
