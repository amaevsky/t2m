using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lingua.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lingua.Services.Rooms.Events
{
    public class RoomMessageSentEvent : BaseRoomEvent
    {
        public Guid MessageId { get; set; }
    }

    public class RoomMessageSentEventHandler :
        INotificationHandler<RoomMessageSentEvent>
    {
        private readonly IEmailService _emailService;
        private readonly ITemplateProvider _templateProvider;
        private readonly ILogger<RoomUpdatesEmailNotificationService> _logger;

        public RoomMessageSentEventHandler(
            IEmailService emailService,
            ITemplateProvider templateProvider,
            ILogger<RoomUpdatesEmailNotificationService> logger = null)
        {
            _emailService = emailService;
            _templateProvider = templateProvider;
            _logger = logger;
        }

        public Task Handle(RoomMessageSentEvent @event, CancellationToken cancellationToken)
        {
            var room = @event.Room;
            var user = room.User(@event.UserId);
            return SendUpdateEmail(room, @event.MessageId, Others(room, user.Id));
        }

        private User[] Others(Room room, Guid userId) => room.Participants.Where(p => p.Id != userId).ToArray();

        private async Task SendUpdateEmail(Room room, Guid messageId, params User[] recipients)
        {
            foreach (var recipient in recipients)
            {
                try
                {
                    var body = await _templateProvider.GetUnreadRoomMessageEmail(room, messageId, recipient);
                    var email = new EmailMessage
                    {
                        Subject = "You Have Unread Messages",
                        Body = body,
                        IsHtml = true,
                    };

                    await _emailService.SendAsync(email, recipient.Email).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, $"Room:{room?.Id} update message is failed to sent to {recipient?.Email}");
                }
            }
        }
    }
}