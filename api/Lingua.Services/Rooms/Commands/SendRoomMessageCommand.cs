using System;
using Lingua.Services.Rooms.Events;
using Lingua.Services.Rooms.Queries;
using Lingua.Shared;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lingua.Services.Rooms.Commands
{
    public class SendRoomMessageCommand : BaseRoomCommand<Room>
    {
        public string Message { get; set; }
    }

    public class SendRoomMessageCommandHandler : IRequestHandler<SendRoomMessageCommand, Room>
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IMediator _mediator;

        public SendRoomMessageCommandHandler(IRoomRepository roomRepository, IMediator mediator)
        {
            _roomRepository = roomRepository;
            _mediator = mediator;
        }

        public async Task<Room> Handle(SendRoomMessageCommand command, CancellationToken cancellationToken)
        {
            var room = await _roomRepository.Get(command.RoomId);
            var message = new Message() {Content = command.Message, AuthorId = command.UserId};
            room.Messages.Add(message);
            await _roomRepository.Update(room);

            _mediator.Publish(new RoomMessageSentEvent()
            {
                Room = room,
                MessageId = message.Id,
                UserId = command.UserId
            }).ConfigureAwait(false);

            return room;
        }
    }
}