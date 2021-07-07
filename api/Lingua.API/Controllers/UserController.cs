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
    public class UserController : ControllerBase
    {
        private readonly Shared.Users.IUserService _userService;

        public UserController(Shared.Users.IUserService userService)
        {
            _userService = userService;
        }


        [HttpGet]
        [Route("me")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var user = await _userService.Get(userId);
            return Ok(user);
        }


        [HttpPut]
        [Route("")]
        public async Task<IActionResult> Update([FromBody]User user)
        {
            await _userService.Update(user);
            return Ok();
        }
    }
}
