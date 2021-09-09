using Lingua.Services.Rooms.Commands;
using Lingua.Shared;
using Lingua.ZoomIntegration;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace Lingua.API.Controllers
{
    public class Event
    {
        public Payload payload { get; set; }
    }

    public class Payload
    {
        public bool user_data_retention { get; set; }
        public string user_id { get; set; }
        public string client_id { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ZoomController : ControllerBase
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;
        private readonly ZoomClientOptions _zoomClientOptions;

        public ZoomController(IRoomRepository roomRepository,
                              IUserRepository userRepository,
                              IOptions<ZoomClientOptions> zoomClientOptions,
                              IMediator mediator)
        {
            _roomRepository = roomRepository;
            _userRepository = userRepository;
            _mediator = mediator;
            _zoomClientOptions = zoomClientOptions.Value;
        }

        [HttpPost]
        [Route("uninstall")]
        public async Task<IActionResult> Uninstall(Event @event)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var header)
                || header.ToString() != _zoomClientOptions.VerificationToken)
            {
                return Unauthorized();
            }

            if (@event.payload.client_id != _zoomClientOptions.ClientId)
            {
                return BadRequest();
            }

            if (@event.payload.user_data_retention == false)
            {
                var userId = (string)@event.payload.user_id;
                var user = (await _userRepository.Get(u => u.ZoomProperties.UserId == userId)).FirstOrDefault();
                var rooms = await _roomRepository.Get(r => r.Participants.Any(p => p.Id == user.Id));
                foreach (var room in rooms)
                {
                    if (room.HostUserId == user.Id)
                    {
                        await _mediator.Send(new RemoveRoomCommand { RoomId = room.Id, UserId = user.Id });
                    }
                    else
                    {
                        await _mediator.Send(new LeaveRoomCommand { RoomId = room.Id, UserId = user.Id });
                    }
                }

                await _userRepository.Remove(user.Id);
            }

            return Ok();
        }
    }
}
