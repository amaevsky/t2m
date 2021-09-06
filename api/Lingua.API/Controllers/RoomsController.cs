using AutoMapper;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Lingua.API.ViewModels;
using Lingua.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Lingua.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ITemplateProvider _templateProvider;
        private readonly IRoomRepository _roomRepository;

        public RoomsController(IRoomService roomService,
                               IMapper mapper,
                               IEmailService emailService,
                               ITemplateProvider templateProvider,
                               IRoomRepository roomRepository)
        {
            _roomService = roomService;
            _mapper = mapper;
            _emailService = emailService;
            _templateProvider = templateProvider;
            _roomRepository = roomRepository;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Available([FromQuery] SearchRoomOptions options)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var rooms = await _roomService.Available(options, userId);

            return Ok(_mapper.Map<List<RoomViewModel>>(rooms));
        }

        [HttpGet]
        [Route("me/upcoming")]
        public async Task<IActionResult> Upcoming()
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var rooms = await _roomService.Upcoming(userId);

            return Ok(_mapper.Map<List<RoomViewModel>>(rooms));
        }

        [HttpGet]
        [Route("me/past")]
        public async Task<IActionResult> Past()
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var rooms = await _roomService.Past(userId);

            return Ok(_mapper.Map<List<RoomViewModel>>(rooms));
        }

        [HttpGet]
        [Route("last")]
        public async Task<IActionResult> Last()
        {
            var rooms = await _roomService.Last();
            return Ok(_mapper.Map<List<RoomViewModel>>(rooms));
        }


        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Create(CreateRoomOptions options)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var room = await _roomService.Create(options, userId);
            var vm = _mapper.Map<RoomViewModel>(room);

            return Ok(vm);
        }

        [HttpPut]
        [Route("")]
        public async Task<IActionResult> Update(UpdateRoomOptions options)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var room = await _roomService.Update(options, userId);

            return Ok();
        }

        [HttpDelete]
        [Route("{roomId}")]
        public async Task<IActionResult> Remove(Guid roomId)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var room = await _roomService.Remove(roomId, userId);

            return Ok();
        }

        [HttpGet]
        [Route("enter/{roomId}")]
        public async Task<IActionResult> Enter(Guid roomId)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var room = await _roomService.Enter(roomId, userId);

            return Ok();
        }

        [HttpGet]
        [Route("leave/{roomId}")]
        public async Task<IActionResult> Leave(Guid roomId)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var room = await _roomService.Leave(roomId, userId);

            return Ok();
        }

        [HttpGet]
        [Route("join/{roomId}")]
        public async Task<IActionResult> Join(Guid roomId)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var room = await _roomService.Join(roomId, userId);

            return Ok(room.JoinUrl);
        }

        [HttpGet]
        [Route("send_calendar_event/{roomId}")]
        public async Task<IActionResult> SendCalendarEvent(Guid roomId)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var room = await _roomRepository.Get(roomId);
            var user = room.Participants.Find(p => p.Id == userId);

            var body = await _templateProvider.GetCalendarEventEmail(user);
            var message = new EmailMessage
            {
                Subject = "Calendar Event",
                Body = body,
                IsHtml = true
            };
            message.Attachments.Add(CreateICSAttachment(room));

            await _emailService.SendAsync(message, user.Email);

            return Ok();
        }

        private System.Net.Mail.Attachment CreateICSAttachment(Room room, bool isCancel = false)
        {
            var e = new CalendarEvent
            {
                Summary = "Talk2Me Room",
                Start = new CalDateTime(room.StartDate),
                End = new CalDateTime(room.EndDate),
                Uid = room.Id.ToString()
            };

            var calendar = new Calendar();
            calendar.Method = "PUBLISH";
            calendar.Events.Add(e);

            if (isCancel)
            {
                calendar.Method = "CANCEL";
                e.Status = "CANCELLED";
                e.Sequence = 1;
            }

            var serializer = new CalendarSerializer();
            var serializedCalendar = serializer.SerializeToString(calendar);
            var bytes = System.Text.Encoding.UTF8.GetBytes(serializedCalendar);
            MemoryStream stream = new MemoryStream(bytes);

            return new System.Net.Mail.Attachment(stream, "room.ics");
        }
    }
}
