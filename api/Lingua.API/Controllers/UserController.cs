using AutoMapper;
using Lingua.API.ViewModels;
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
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, IMapper mapper, IEmailService emailService, ITemplateProvider templateProvider)
        {
            _userRepository = userRepository;
            _mapper = mapper;
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
            return Ok(_mapper.Map<UserViewModel>(user));
        }


        [HttpPut]
        [Route("")]
        public async Task<IActionResult> Update([FromBody] UserViewModel userModel)
        {
            var current = await _userRepository.Get(userModel.Id);
            var isReady = current.IsReady;

            //override current object here
            var user = _mapper.Map(userModel, current);
            await _userRepository.Update(user);

            if (!isReady)
            {
                var body = await _templateProvider.GetWelcomeLetterEmail(user);
                _emailService.SendAsync(
                    new EmailMessage
                    {
                        Subject = "Welcome!",
                        Body = body,
                        IsHtml = true
                    }, user.Email).ConfigureAwait(false);
            }

            return Ok();
        }
    }
}
