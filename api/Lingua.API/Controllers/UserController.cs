using Lingua.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Lingua.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        [HttpGet]
        [Route("me")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var user = await _userRepository.Get(userId);
            return Ok(user);
        }


        [HttpPut]
        [Route("")]
        public async Task<IActionResult> Update([FromBody]User user)
        {
            await _userRepository.Update(user);
            return Ok();
        }
    }
}
