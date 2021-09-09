using Lingua.Services.Rooms.Events;
using Lingua.Shared;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Lingua.Services.Rooms.Commands
{
    public class LeaveRoomCommand: BaseRoomCommand<Room>
    {

    }

    public class LeaveRoomCommandHandler : IRequestHandler<LeaveRoomCommand, Room>
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;

        public LeaveRoomCommandHandler(IRoomRepository roomRepository, IUserRepository userRepository, IMediator mediator)
        {
            _roomRepository = roomRepository;
            _userRepository = userRepository;
            _mediator = mediator;
        }

        public async Task<Room> Handle(LeaveRoomCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Get(command.UserId);
            var room = await _roomRepository.Get(command.RoomId);

            room.Participants.RemoveAll(p => p.Id == user.Id);
            await _roomRepository.Update(room);

            _mediator.Publish(new RoomLeftEvent { Room = room, User = user }).ConfigureAwait(false);

            return room;
        }
    }
}
