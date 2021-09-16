using System;
using System.Linq;
using Lingua.Services.Rooms.Events;
using Lingua.Shared;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Lingua.Services.Rooms.Queries;

namespace Lingua.Services.Rooms.Commands
{
    public class UpdateRoomCommand : BaseRoomCommand<Room>
    {
        public Guid RoomId { get; set; }
        public UpdateRoomOptions Options { get; set; }
    }

    public class UpdateRoomCommandHandler : IRequestHandler<UpdateRoomCommand, Room>
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IMediator _mediator;

        public UpdateRoomCommandHandler(IRoomRepository roomRepository, IMediator mediator)
        {
            _roomRepository = roomRepository;
            _mediator = mediator;
        }

        public async Task<Room> Handle(UpdateRoomCommand command, CancellationToken cancellationToken)
        {
            var options = command.Options;
            var room = await _roomRepository.Get(command.RoomId);
            var copy = (Room)room.Clone();

            var timeChanged = room.StartDate != options.StartDate ||
                              room.DurationInMinutes != options.DurationInMinutes;

            var updated = timeChanged || room.Topic != options.Topic;
            
            room.Topic = options.Topic;
            room.StartDate = options.StartDate;
            room.DurationInMinutes = options.DurationInMinutes;
            room.EndDate = options.StartDate.AddMinutes(options.DurationInMinutes);

            if (timeChanged)
            {
                var user = room.User(command.UserId);
                var others = room.Participants
                    .Where(p => p.Id != command.UserId)
                    .Select(p => room.User(p.Id));

                var conflicts = await _mediator.Send(
                    new ConflictingRoomsQuery()
                    {
                        Start = room.StartDate,
                        End = room.EndDate,
                        UserId = user.Id
                    });

                if (conflicts.Any(c=> c.Id != room.Id))
                {
                    throw new ValidationException(ValidationExceptionType.Rooms_Update_Conflict_User);
                }

                foreach (var other in others)
                {
                    conflicts = await _mediator.Send(
                        new ConflictingRoomsQuery()
                        {
                            Start = room.StartDate,
                            End = room.EndDate,
                            UserId = other.Id
                        });

                    if (conflicts.Any(c=> c.Id != room.Id))
                    {
                        throw new ValidationException(ValidationExceptionType.Rooms_Update_Conflict_Roommate);
                    }
                }
            }

            if (updated)
            {
                await _roomRepository.Update(room);
                _mediator.Publish(new RoomUpdatedEvent {Room = room, PreviousVersion = copy, User = room.User(command.UserId)})
                    .ConfigureAwait(false);
            }

            return room;
        }
    }
}