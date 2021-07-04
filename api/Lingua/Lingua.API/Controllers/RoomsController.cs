using Lingua.Shared;
using Lingua.ZoomIntegration;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Lingua.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly IMeetingService _zoomMeetingService;
        private readonly IRoomService _roomService;
        private readonly Shared.Users.IUserService _userService;

        public RoomsController(IMeetingService zoomMeetingService,
                               IRoomService roomService,
                               Shared.Users.IUserService userService)
        {
            _zoomMeetingService = zoomMeetingService;
            _roomService = roomService;
            _userService = userService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> All()
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var user = await _userService.Get(userId);

            var searchRoomOptions = new SearchRoomOptions { Language = user.TargetLanguage };
            var rooms = await _roomService.Get(searchRoomOptions);
            return Ok(rooms);
        }


        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Create(CreateRoomOptions createRoomOptions)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var user = await _userService.Get(userId);
            createRoomOptions.Host = user;

            var room = await _roomService.Create(createRoomOptions);
            return Ok(room);
        }

        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> Update(UpdateRoomOptions updateRoomOptions)
        {
            await _roomService.Update(updateRoomOptions);
            return Ok();
        }

        [HttpPost]
        [Route("join")]
        public async Task<IActionResult> Join(Guid roomId)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var user = await _userService.Get(userId);

            await _roomService.AddParticipant(roomId, user);
            return Ok();
        }

        [HttpPost]
        [Route("start")]
        public async Task<IActionResult> Start(Guid roomId)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var user = await _userService.Get(userId);

            if (user.ZoomProperties == null)
            {
                return BadRequest();
            }

            var room = await _roomService.Get(roomId);

            var request = new CreateMeetingRequest
            {
                Topic = room.Topic,
                Duration = (int)room.Duration.Value.TotalMilliseconds,
                StartTime = room.StartDate.Value,
                Type = MeetingType.Scheduled
            };

            var meeting = await _zoomMeetingService.CreateMeeting(user.ZoomProperties.AccessToken, request);
            room.JoinUrl = meeting.JoinUrl;
            //save();

            return Ok();
        }
    }
}
