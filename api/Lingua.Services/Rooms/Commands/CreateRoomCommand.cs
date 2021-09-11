using Lingua.Services.Rooms.Events;
using Lingua.Services.Rooms.Queries;
using Lingua.Shared;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lingua.Services.Rooms.Commands
{
    public class CreateRoomCommand : BaseRoomCommand<Room>
    {
        public CreateRoomOptions Options { get; set; }
    }

    public class CreateRoomCommandHandler : IRequestHandler<CreateRoomCommand, Room>
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;

        public CreateRoomCommandHandler(IRoomRepository roomRepository, IUserRepository userRepository, IMediator mediator)
        {
            _roomRepository = roomRepository;
            _userRepository = userRepository;
            _mediator = mediator;
        }

        public async Task<Room> Handle(CreateRoomCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Get(command.UserId);
            var start = command.Options.StartDate;
            var end = command.Options.StartDate.AddMinutes(command.Options.DurationInMinutes);

            var conflicts = await _mediator.Send(new ConflictingRoomsQuery { UserId = command.UserId, Start = start, End = end });

            if (conflicts.Any())
            {
                throw new ValidationException(ValidationExceptionType.Rooms_Create_Conflict);
            }

            var room = new Room
            {
                HostUserId = command.UserId,
                Language = command.Options.Language,
                LanguageLevel = user.LanguageLevel,
                StartDate = start,
                EndDate = end,
                DurationInMinutes = command.Options.DurationInMinutes,
                Topic = command.Options.Topic,
                MaxParticipants = 2
            };

            room.Participants.Add(user);

            await _roomRepository.Create(room);

            _mediator.Publish(new RoomCreatedEvent { Room = room, UserId = command.UserId }).ConfigureAwait(false);

            return room;
        }
    }
}
