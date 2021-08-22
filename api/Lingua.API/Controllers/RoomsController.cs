using AutoMapper;
using Lingua.API.ViewModels;
using Lingua.Shared;
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
        private readonly IRoomService _roomService;
        private readonly IMapper _mapper;

        public RoomsController(IRoomService roomService, IMapper mapper)
        {
            _roomService = roomService;
            _mapper = mapper;
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
    }
}
