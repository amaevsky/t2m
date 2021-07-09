using Lingua.API.Realtime;
using Lingua.Shared;
using Lingua.ZoomIntegration;
using Lingua.ZoomIntegration.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace Lingua.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly IMeetingService _zoomMeetingService;
        private readonly IRoomService _roomService;
        private readonly Shared.Users.IUserService _userService;
        private readonly ITokenProvider _tokenProvider;
        private readonly IHubContext<RoomsHub, IRoomsRealtimeClient> _roomsHub;

        public RoomsController(IMeetingService zoomMeetingService,
                               IRoomService roomService,
                               Shared.Users.IUserService userService,
                               ITokenProvider tokenProvider,
                               IHubContext<RoomsHub, IRoomsRealtimeClient> roomsHub
                               )
        {
            _zoomMeetingService = zoomMeetingService;
            _roomService = roomService;
            _userService = userService;
            _tokenProvider = tokenProvider;
            _roomsHub = roomsHub;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Available([FromQuery] SearchRoomOptions options)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var user = await _userService.Get(userId);

            var rooms = (await _roomService.Get(r =>
                                    r.StartDate > DateTime.UtcNow
                                    && r.Language == user.TargetLanguage
                                    && !r.Participants.Any(p => p.Id == userId)))
                .Where(r => r.Participants.Count < r.MaxParticipants);

            if (options != null)
            {
                rooms = ApplySearchFilter(options, user, rooms);
            }

            return Ok(rooms.ToList());
        }

        private static IEnumerable<Room> ApplySearchFilter(SearchRoomOptions options, User user, IEnumerable<Room> rooms)
        {
            if (options?.Levels?.Any() == true)
            {
                rooms = rooms.Where(r => options.Levels.Contains(r.Host.LanguageLevel));
            }

            if (options?.Days?.Any() == true)
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById(TZConvert.IanaToWindows(user.Timezone));
                rooms = rooms.Where(r => options.Days.Contains(TimeZoneInfo.ConvertTimeFromUtc(r.StartDate.Value, tz).DayOfWeek));
            }

            if ((bool)options?.TimeFrom.HasValue && (bool)options?.TimeTo.HasValue)
            {
                //store room.till date
                rooms = rooms.Where(r => r.StartDate.Value.TimeOfDay > options.TimeFrom.Value.TimeOfDay
                                        && r.StartDate.Value.TimeOfDay < options.TimeTo.Value.TimeOfDay);
            }

            return rooms;
        }

        [HttpGet]
        [Route("me/upcoming")]
        public async Task<IActionResult> Upcoming()
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var user = await _userService.Get(userId);

            var start = DateTime.UtcNow.AddMinutes(-60);
            var rooms = (await _roomService.Get(r =>
                            r.StartDate > start
                            && r.Participants.Any(p => p.Id == userId))
                ).Where(r => r.StartDate > DateTime.UtcNow.AddMinutes(-1 * r.DurationInMinutes.Value));

            return Ok(rooms.ToList());
        }


        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Create(CreateRoomOptions options)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var user = await _userService.Get(userId);

            var room = new Room
            {
                HostUserId = userId,
                Language = options.Language,
                StartDate = options.StartDate,
                DurationInMinutes = options.DurationInMinutes,
                Topic = options.Topic,
                MaxParticipants = 2
            };

            room.Participants.Add(user);

            await _roomService.Create(room);
            await _roomsHub.Clients.All.OnAdd(room, userId);

            return Ok(room);
        }

        [HttpPut]
        [Route("")]
        public async Task<IActionResult> Update(UpdateRoomOptions options)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

            var room = await _roomService.Get(options.RoomId);
            room.Topic = options.Topic;
            room.StartDate = options.Date;

            await _roomService.Update(room);
            await _roomsHub.Clients.All.OnUpdate(room, userId);

            return Ok();
        }

        [HttpDelete]
        [Route("{roomId}")]
        public async Task<IActionResult> Remove(Guid roomId)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var room = await _roomService.Get(roomId);
            await _roomService.Remove(roomId);
            await _roomsHub.Clients.All.OnRemove(room, userId);

            return Ok();
        }

        [HttpGet]
        [Route("enter/{roomId}")]
        public async Task<IActionResult> Enter(Guid roomId)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var user = await _userService.Get(userId);
            var room = await _roomService.Get(roomId);

            if (room.Participants.Count == room.MaxParticipants)
            {
                throw new Exception("Room is already full");
            }

            room.Participants.Add(user);
            await _roomService.Update(room);
            await _roomsHub.Clients.All.OnEnter(room, userId);

            return Ok();
        }

        [HttpGet]
        [Route("leave/{roomId}")]
        public async Task<IActionResult> Leave(Guid roomId)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var user = await _userService.Get(userId);

            var room = await _roomService.Get(roomId);
            room.Participants.RemoveAll(p => p.Id == user.Id);
            await _roomService.Update(room);
            await _roomsHub.Clients.All.OnLeave(room, userId);

            return Ok();
        }

        [HttpGet]
        [Route("join/{roomId}")]
        public async Task<IActionResult> Join(Guid roomId)
        {
            var room = await _roomService.Get(roomId);
            if (room.JoinUrl != null)
            {
                return Ok(room.JoinUrl);
            }

            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var user = await _userService.Get(userId);
            var accessTokens = user.ZoomProperties?.AccessTokens;

            if (accessTokens == null)
            {
                return BadRequest();
            }

            var updated = await _tokenProvider.UseToken(accessTokens, async (accessTokens) =>
            {

                var request = new CreateMeetingRequest
                {
                    Topic = room.Topic,
                    Duration = room.DurationInMinutes.Value,
                    StartTime = room.StartDate.Value,
                    Type = MeetingType.Scheduled,
                    Timezone = "UTC"
                };

                var meeting = await _zoomMeetingService.CreateMeeting(accessTokens.AccessToken, request);
                room.JoinUrl = meeting.JoinUrl;
                await _roomService.Update(room);
            });

            if (updated)
            {
                await _userService.Update(user);
            }

            return Ok(room.JoinUrl);

        }
    }
}
