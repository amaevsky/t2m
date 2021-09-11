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
        private readonly IMediator _mediator;

        public RemoveRoomCommandHandler(IRoomRepository roomRepository, IMediator mediator)
        {
            _roomRepository = roomRepository;
            _mediator = mediator;
        }

        public async Task<Room> Handle(RemoveRoomCommand command, CancellationToken cancellationToken)
        {
            await _roomRepository.Remove(command.RoomId);
            var room = await _roomRepository.Get(command.RoomId);

            _mediator.Publish(new RoomRemovedEvent { Room = room, UserId = command.UserId }).ConfigureAwait(false);

            return room;
        }
    }
}
