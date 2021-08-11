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

        public UserController(IUserRepository userRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
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
                var body = $@"
<html>
<body>

<p>Hi {user.Firstname} 👋<p>
<br/>
<p>Welcome and thanks for joining Talk2Me! We hope you will enjoy practicing language with us. Here are 4 steps to help start your journey:<p>
<p>
<div>1️⃣ Go to <a href={"https://t2m.app/rooms/find"}>Find a Room page</a> and enter any room which works for you.</div>
<div>2️⃣ If you didn’t find the appropriate room, press <u>'Create a Room' button</u>, make your own room and wait until someone enters your room or invite you friends to practice together.</div>
<div>3️⃣ Wait until the time comes and join the call.</div>
<div>4️⃣ Practice language. Ask questions. Use new words. And have fun!</div>
</p>
<p>⚠ If you have any questions or need help, please feel free to <a href={"https://t2m.app/help/contact-us"}>contact us here</a>.</b><p>
<br/>
<p>
<div>Best Regards,</div>
<div>Talk2Me App Team</div>
</p>

</body>
</html>
";
                _emailService.SendAsync("Welcome!", body, true, user.Email).ConfigureAwait(false);
            }

            return Ok();
        }
    }
}
