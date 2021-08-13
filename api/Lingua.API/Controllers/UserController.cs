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
        private readonly IEmailService _emailService;
        private readonly ITemplateProvider _templateProvider;

        public UserController(IUserRepository userRepository, IEmailService emailService, ITemplateProvider templateProvider)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _templateProvider = templateProvider;
        }


        [HttpGet]
        [Route("me")]
        public async Task<IActionResult> GetProfile()
        {
            if (!HttpContext.User.Claims.Any())
            {
                return Unauthorized();
            }

            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var user = await _userRepository.Get(userId);
            return Ok(user);
        }


        [HttpPut]
        [Route("")]
        public async Task<IActionResult> Update([FromBody] User user)
        {
            var current = await _userRepository.Get(user.Id);
            await _userRepository.Update(user);

            if (!current.IsReady)
            {
                var body = await _templateProvider.GetWelcomeLetterEmail(user);
                _emailService.SendAsync("Welcome!", body, true, user.Email).ConfigureAwait(false);
            }

            return Ok();
        }
    }
}
