using Lingua.Services.Rooms.Events;
using Lingua.Shared;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Lingua.Services.Rooms.Commands
{
    public class UpdateRoomCommand : BaseRoomCommand<Room>
    {
        public UpdateRoomOptions Options { get; set; }
    }

    public class UpdateRoomCommandHandler : IRequestHandler<UpdateRoomCommand, Room>
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;

        public UpdateRoomCommandHandler(IRoomRepository roomRepository, IUserRepository userRepository, IMediator mediator)
        {
            _roomRepository = roomRepository;
            _userRepository = userRepository;
            _mediator = mediator;
        }

        public async Task<Room> Handle(UpdateRoomCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Get(command.UserId);
            var options = command.Options;
            var room = await _roomRepository.Get(options.RoomId);
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

            await _roomRepository.Update(room);
            _mediator.Publish(new RoomUpdatedEvent { Room = room, User = user }).ConfigureAwait(false);

            return room;
        }
    }
}
