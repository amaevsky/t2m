using Lingua.Services.Rooms.Events;
using Lingua.Shared;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lingua.Services
{
    public class RoomUpdatesEmailNotificationService :
        INotificationHandler<RoomEnteredEvent>,
        INotificationHandler<RoomLeftEvent>,
        INotificationHandler<RoomRemovedEvent>
    {
        private readonly IEmailService _emailService;
        private readonly ITemplateProvider _templateProvider;
        private readonly ILogger<RoomUpdatesEmailNotificationService> _logger;

        public RoomUpdatesEmailNotificationService(
                            IEmailService emailService,
                            ITemplateProvider templateProvider,
                            ILogger<RoomUpdatesEmailNotificationService> logger = null)
        {
            _emailService = emailService;
            _templateProvider = templateProvider;
            _logger = logger;
        }

        public Task Handle(RoomEnteredEvent @event, CancellationToken cancellationToken)
        {
            var room = @event.Room;
            var user = @event.User;
            return SendUpdateEmail(room, $"<b>{user.Fullname} entered the room.</b>", Others(room, user.Id));
        }

        public Task Handle(RoomLeftEvent @event, CancellationToken cancellationToken)
        {
            var room = @event.Room;
            var user = @event.User;
            return SendUpdateEmail(room, $"<b>{user.Fullname} left the room.</b>", Others(room, user.Id));
        }

        public Task Handle(RoomRemovedEvent @event, CancellationToken cancellationToken)
        {
            var room = @event.Room;
            var user = @event.User;
            return SendUpdateEmail(room, $"<b>{user.Fullname} deleted the room you have previously entered.</b> You can go ahead and create your own room for that time.", Others(room, user.Id));
        }

        private User[] Others(Room room, Guid userId) => room.Participants.Where(p => p.Id != userId).ToArray();

        private async Task SendUpdateEmail(Room room, string message, params User[] recipients)
        {
            foreach (var recipient in recipients)
            {
                try
                {
                    _logger?.LogInformation($"Before room:{room?.Id} update message is sent to {recipient?.Email}");

                    var body = await _templateProvider.GetRoomUpdateEmail(message, room, recipient);
                    var email = new EmailMessage
                    {
                        Subject = "Room Update",
                        Body = body,
                        IsHtml = true,
                    };

                    await _emailService.SendAsync(email, recipient.Email).ConfigureAwait(false);

                    _logger?.LogInformation($"After room:{room?.Id} update message is sent to {recipient?.Email}");
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, $"Room:{room?.Id} update message is failed to sent to {recipient?.Email}");
                }
            }
        }
    }
}
