using Lingua.Services.Rooms.Events;
using Lingua.Services.Rooms.Queries;
using Lingua.Shared;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lingua.Services.Rooms.Commands
{
    public class EnterRoomCommand: BaseRoomCommand<Room>
    {

    }

    public class EnterRoomCommandHandler : IRequestHandler<EnterRoomCommand, Room>
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;
        private readonly IDateTimeProvider _dateTime;

        public EnterRoomCommandHandler(IRoomRepository roomRepository, IUserRepository userRepository, IMediator mediator, IDateTimeProvider dateTime)
        {
            _roomRepository = roomRepository;
            _userRepository = userRepository;
            _mediator = mediator;
            _dateTime = dateTime;
        }

        public async Task<Room> Handle(EnterRoomCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Get(command.UserId);
            var room = await _roomRepository.Get(command.RoomId);

            if (room.StartDate < _dateTime.UtcNow)
            {
                throw new ValidationException(ValidationExceptionType.Rooms_Enter_AlreadyStarted);
            }
            if (room.Participants.Any(p => p.Id == command.UserId))
            {
                return room;
            }
            if (room.Participants.Count == room.MaxParticipants)
            {
                throw new ValidationException(ValidationExceptionType.Rooms_Enter_AlreadyFull);
            }

            var start = room.StartDate;
            var end = room.StartDate.AddMinutes(room.DurationInMinutes);

            var conflicts = await _mediator.Send(new ConflictingRoomsQuery { UserId = command.UserId, Start = start, End = end });

            if (conflicts.Any())
            {
                throw new ValidationException(ValidationExceptionType.Rooms_Enter_Conflict);
            }

            room.Participants.Add(user);
            room.Updated = _dateTime.UtcNow;
            await _roomRepository.Update(room);

            _mediator.Publish(new RoomEnteredEvent { Room = room, User = user }).ConfigureAwait(false);

            return room;
        }
    }
}
