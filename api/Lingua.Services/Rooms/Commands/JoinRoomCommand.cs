using Lingua.Services.Rooms.Events;
using Lingua.Shared;
using Lingua.ZoomIntegration;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Lingua.Services.Rooms.Commands
{
    public class JoinRoomCommand : BaseRoomCommand<Room>
    {

    }

    public class JoinRoomCommandHandler : IRequestHandler<JoinRoomCommand, Room>
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMeetingClient _zoomMeetingClient;
        private readonly IMediator _mediator;

        public JoinRoomCommandHandler(IRoomRepository roomRepository, IUserRepository userRepository, IMeetingClient zoomMeetingClient, IMediator mediator)
        {
            _roomRepository = roomRepository;
            _userRepository = userRepository;
            _zoomMeetingClient = zoomMeetingClient;
            _mediator = mediator;
        }

        public async Task<Room> Handle(JoinRoomCommand command, CancellationToken cancellationToken)
        {
            var room = await _roomRepository.Get(command.RoomId);
            if (room.JoinUrl != null)
            {
                return room;
            }

            var user = room.User(command.UserId);
            var accessTokens = user.ZoomProperties?.AccessTokens;

            var request = new CreateMeetingRequest
            {
                Topic = room.Topic,
                Duration = room.DurationInMinutes,
                StartTime = room.StartDate,
                Type = MeetingType.Scheduled,
                Timezone = "UTC"
            };

            var response = await _zoomMeetingClient.CreateMeeting(accessTokens, request);
            room.JoinUrl = response.Response.JoinUrl;
            await _roomRepository.Update(room);

            if (response.NewTokens != null)
            {
                await _userRepository.Update(user);
            }

            _mediator.Publish(new RoomJoinedEvent { Room = room, UserId = command.UserId }).ConfigureAwait(false);
            return room;
        }
    }
}
