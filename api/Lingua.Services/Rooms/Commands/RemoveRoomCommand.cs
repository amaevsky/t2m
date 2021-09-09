using Lingua.Services.Rooms.Events;
using Lingua.Shared;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Lingua.Services.Rooms.Commands
{
    public class RemoveRoomCommand : BaseRoomCommand<Room>
    {

    }

    public class RemoveRoomCommandHandler : IRequestHandler<RemoveRoomCommand, Room>
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;

        public RemoveRoomCommandHandler(IRoomRepository roomRepository, IUserRepository userRepository, IMediator mediator)
        {
            _roomRepository = roomRepository;
            _userRepository = userRepository;
            _mediator = mediator;
        }

        public async Task<Room> Handle(RemoveRoomCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Get(command.UserId);
            await _roomRepository.Remove(command.RoomId);
            var room = await _roomRepository.Get(command.RoomId);

            _mediator.Publish(new RoomRemovedEvent { Room = room, User = user }).ConfigureAwait(false);

            return room;
        }
    }
}
