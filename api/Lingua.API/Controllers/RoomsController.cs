using AutoMapper;
using Lingua.API.ViewModels;
using Lingua.Services.Rooms.Commands;
using Lingua.Services.Rooms.Queries;
using Lingua.Shared;
using MediatR;
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
    public class RoomsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public RoomsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Available([FromQuery] SearchRoomOptions options)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var rooms = await _mediator.Send(new AvailableRoomsQuery { Options = options, UserId = userId });

            return Ok(_mapper.Map<List<RoomViewModel>>(rooms));
        }

        [HttpGet]
        [Route("me/upcoming")]
        public async Task<IActionResult> Upcoming()
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var rooms = await _mediator.Send(new UpcomingRoomsQuery { UserId = userId });

            return Ok(_mapper.Map<List<RoomViewModel>>(rooms));
        }

        [HttpGet]
        [Route("me/past")]
        public async Task<IActionResult> Past()
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var rooms = await _mediator.Send(new PastRoomsQuery { UserId = userId });

            return Ok(_mapper.Map<List<RoomViewModel>>(rooms));
        }

        [HttpGet]
        [Route("last")]
        public async Task<IActionResult> Last()
        {
            var rooms = await _mediator.Send(new RecentlyEnteredRoomsQuery());
            return Ok(_mapper.Map<List<RoomViewModel>>(rooms));
        }


        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Create(CreateRoomOptions options)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var room = await _mediator.Send(new CreateRoomCommand { Options = options, UserId = userId });
            var vm = _mapper.Map<RoomViewModel>(room);

            return Ok(vm);
        }

        [HttpPut]
        [Route("")]
        public async Task<IActionResult> Update(UpdateRoomOptions options)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            await _mediator.Send(new UpdateRoomCommand { Options = options, UserId = userId });

            return Ok();
        }

        [HttpDelete]
        [Route("{roomId}")]
        public async Task<IActionResult> Remove(Guid roomId)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            await _mediator.Send(new CreateRoomCommand { RoomId = roomId, UserId = userId });

            return Ok();
        }

        [HttpGet]
        [Route("enter/{roomId}")]
        public async Task<IActionResult> Enter(Guid roomId)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            await _mediator.Send(new EnterRoomCommand { RoomId = roomId, UserId = userId }); ;

            return Ok();
        }

        [HttpGet]
        [Route("leave/{roomId}")]
        public async Task<IActionResult> Leave(Guid roomId)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            await _mediator.Send(new LeaveRoomCommand { RoomId = roomId, UserId = userId });

            return Ok();
        }

        [HttpGet]
        [Route("join/{roomId}")]
        public async Task<IActionResult> Join(Guid roomId)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var room = await _mediator.Send(new JoinRoomCommand { RoomId = roomId, UserId = userId });

            return Ok(room.JoinUrl);
        }

        [HttpGet]
        [Route("send_calendar_event/{roomId}")]
        public async Task<IActionResult> SendCalendarEvent(Guid roomId)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

            await _mediator.Send(new SendCalendarEventCommand { RoomId = roomId, UserId = userId });

            return Ok();
        }
    }
}
