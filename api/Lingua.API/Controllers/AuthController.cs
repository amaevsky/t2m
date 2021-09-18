using Lingua.Shared;
using Lingua.ZoomIntegration;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Lingua.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthClient _zoomAuthService;
        private readonly IUserClient _zoomUserService;
        private readonly IUserRepository _userRepository;
        private readonly JwtOptions _jwtOptions;

        public AuthController(IAuthClient zoomAuthService, IUserClient zoomUserService, IUserRepository userRepository,
            IOptions<JwtOptions> jwtOptions)
        {
            _zoomAuthService = zoomAuthService;
            _zoomUserService = zoomUserService;
            _userRepository = userRepository;
            _jwtOptions = jwtOptions.Value;
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
                    Firstname = zoomUser.FirstName,
                    Lastname = zoomUser.LastName,
                    AvatarUrl = zoomUser.PicUrl,
                    ZoomProperties = new ZoomProperties
                    {
                        UserId = zoomUser.Id,
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

            var jwtToken = new JwtSecurityToken(
                _jwtOptions.Issuer,
                claims: claims,
                expires: _jwtOptions.Expiration.HasValue ? DateTime.UtcNow.AddDays(_jwtOptions.Expiration.Value) : null,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.EncryptionKey)),
                    SecurityAlgorithms.HmacSha256Signature));

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return Ok(new {isNewAccount, accessToken});
        }

        [HttpGet]
        [Route("login")]
        public async Task<IActionResult> Login()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                    .Value);
                var user = await _userRepository.Get(userId);
                return Ok();
            }

            return StatusCode(403);
        }

        [HttpGet]
        [Route("logout")]
        public IActionResult Logout()
        {
            return Ok();
        }
    }
}