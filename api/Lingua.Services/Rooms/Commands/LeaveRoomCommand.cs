using Lingua.Services.Rooms.Events;
using Lingua.Shared;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Lingua.Services.Rooms.Commands
{
    public class LeaveRoomCommand : BaseRoomCommand<Room>
    {
    }

    public class LeaveRoomCommandHandler : IRequestHandler<LeaveRoomCommand, Room>
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IMediator _mediator;

        public LeaveRoomCommandHandler(IRoomRepository roomRepository, IMediator mediator)
        {
            _roomRepository = roomRepository;
            _mediator = mediator;
        }

        public async Task<Room> Handle(LeaveRoomCommand command, CancellationToken cancellationToken)
        {
            var room = await _roomRepository.Get(command.RoomId);
            var user = room.User(command.UserId);
            room.Participants.RemoveAll(p => p.Id == command.UserId);
            room.Messages.Clear();

            await _roomRepository.Update(room);

            _mediator.Publish(new RoomLeftEvent {Room = room, User = user}).ConfigureAwait(false);

            return room;
        }
    }
}