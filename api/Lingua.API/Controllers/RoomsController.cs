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
        private readonly IEmailService _emailService;

        public RoomsController(IMeetingService zoomMeetingService,
                               IRoomService roomService,
                               Shared.Users.IUserService userService,
                               ITokenProvider tokenProvider,
                               IHubContext<RoomsHub, IRoomsRealtimeClient> roomsHub,
                               IEmailService emailService
                               )
        {
            _zoomMeetingService = zoomMeetingService;
            _roomService = roomService;
            _userService = userService;
            _tokenProvider = tokenProvider;
            _roomsHub = roomsHub;
            _emailService = emailService;
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
                rooms = ApplySearchFilter(options, rooms);
            }

            return Ok(rooms.ToList());
        }

        private static IEnumerable<Room> ApplySearchFilter(SearchRoomOptions options, IEnumerable<Room> rooms)
        {
            if (options?.Levels?.Any() == true)
            {
                rooms = rooms.Where(r => options.Levels.Contains(r.Host.LanguageLevel));
            }

            if (options?.Days?.Any() == true)
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById(TZConvert.IanaToWindows(options.Timezone));
                rooms = rooms.Where(r => options.Days.Contains(TimeZoneInfo.ConvertTimeFromUtc(r.StartDate, tz).DayOfWeek));
            }

            if ((bool)options?.TimeFrom.HasValue && (bool)options?.TimeTo.HasValue)
            {
                //store room.till date
                rooms = rooms.Where(r => r.StartDate.TimeOfDay > options.TimeFrom.Value.TimeOfDay
                                        && r.StartDate.TimeOfDay < options.TimeTo.Value.TimeOfDay);
            }

            return rooms;
        }

        [HttpGet]
        [Route("me/upcoming")]
        public async Task<IActionResult> Upcoming()
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var user = await _userService.Get(userId);

            var rooms = await _roomService.Get(r =>
                            r.Participants.Any(p => p.Id == userId)
                            && (
                                (r.EndDate > DateTime.UtcNow && r.Participants.Count > 1)
                                || r.StartDate > DateTime.UtcNow
                               )
                            );

            return Ok(rooms.ToList());
        }


        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Create(CreateRoomOptions options)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var user = await _userService.Get(userId);

            var start = options.StartDate;
            var end = options.StartDate.AddMinutes(options.DurationInMinutes);

            var conflicts = await _roomService.Get(r =>
                                r.StartDate < end
                                && start < r.EndDate
                                && r.Participants.Any(p => p.Id == userId));

            if (conflicts.Any())
            {
                return BadRequest("You have conflicting rooms for this time frame");
            }

            var room = new Room
            {
                HostUserId = userId,
                Language = options.Language,
                StartDate = start,
                EndDate = end,
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
            if (options.StartDate.HasValue)
            {
                room.StartDate = options.StartDate.Value;
                room.EndDate = options.StartDate.Value.AddMinutes(room.DurationInMinutes);
            }

            if (options.DurationInMinutes.HasValue)
            {
                room.DurationInMinutes = options.DurationInMinutes.Value;
                room.EndDate = room.StartDate.AddMinutes(options.DurationInMinutes.Value);
            }

            await _roomService.Update(room);
            await _roomsHub.Clients.All.OnUpdate(room, userId);
            await _emailService.SendAsync("Test", "Test", room.Participants.Select(p => p.Email).ToArray());

            return Ok();
        }

        [HttpDelete]
        [Route("{roomId}")]
        public async Task<IActionResult> Remove(Guid roomId)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var room = await _roomService.Get(roomId);
            room.IsRemoved = true;
            await _roomService.Update(room);
            await _roomsHub.Clients.All.OnRemove(room, userId);
            await _emailService.SendAsync("Test", "Test", room.Participants.Select(p => p.Email).ToArray());

            return Ok();
        }

        [HttpGet]
        [Route("enter/{roomId}")]
        public async Task<IActionResult> Enter(Guid roomId)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var user = await _userService.Get(userId);
            var room = await _roomService.Get(roomId);

            if (room.StartDate < DateTime.UtcNow)
            {
                return BadRequest("This room is already started.");
            }
            if (!room.Participants.Any(p => p.Id == userId))
            {
                return BadRequest("You have already entered this room.");
            }
            if (room.Participants.Count == room.MaxParticipants)
            {
                return BadRequest("This room is already full.");
            }

            var start = room.StartDate;
            var end = room.StartDate.AddMinutes(room.DurationInMinutes);

            var conflicts = await _roomService.Get(r =>
                                r.StartDate < end
                                && start < r.EndDate
                                && r.Participants.Any(p => p.Id == userId));

            if (conflicts.Any())
            {
                return BadRequest("You have conflicting rooms for this time frame");
            }

            if (room.Participants.Count == room.MaxParticipants)
            {
                return BadRequest("Room is already full");
            }

            room.Participants.Add(user);
            await _roomService.Update(room);
            await _roomsHub.Clients.All.OnEnter(room, userId);
            await _emailService.SendAsync("Test", "Test", room.Participants.Select(p => p.Email).ToArray());

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
            await _emailService.SendAsync("Test", "Test", room.Participants.Select(p => p.Email).ToArray());

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
                    Duration = room.DurationInMinutes,
                    StartTime = room.StartDate,
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
