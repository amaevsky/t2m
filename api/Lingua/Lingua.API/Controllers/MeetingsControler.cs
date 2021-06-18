using Lingua.ZoomIntegration;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Lingua.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MeetingsControler : ControllerBase
    {
        private readonly IMeetingService _zoomMeetingService;

        public MeetingsControler(IMeetingService zoomMeetingService)
        {
            _zoomMeetingService = zoomMeetingService;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> ZoomLogin()
        {
            var accessToken = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ZoomAccessToken")?.Value;
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(userId))
            {
                return BadRequest();
            }

            var options = new MeetingOptions
            {

            };

            var request = new CreateMeetingRequest
            {
                Topic = "test",
                Duration = 60,
                StartTime = System.DateTime.UtcNow.AddMinutes(5),
                Type = MeetingType.Scheduled
            };

            await _zoomMeetingService.CreateMeeting(accessToken, userId, request);

            return Ok();
        }
    }
}
