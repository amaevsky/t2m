using Lingua.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Lingua.Services
{
    public class RoomUpdatesEmailNotificationService : IRoomUpdatesObserver
    {
        private readonly IEmailService _emailService;
        private readonly ITemplateProvider _templateProvider;
        private readonly ILogger<RoomService> _logger;

        public RoomUpdatesEmailNotificationService(
                            IEmailService emailService,
                            ITemplateProvider templateProvider,
                            ILogger<RoomService> logger = null)
        {
            _emailService = emailService;
            _templateProvider = templateProvider;
            _logger = logger;
        }

        public Task OnUpdate(RoomUpdateType updateType, Room room, User user)
        {
            User[] others(Room room, Guid userId) => room.Participants.Where(p => p.Id != userId).ToArray();

            switch (updateType)
            {
                case RoomUpdateType.Created:
                    break;
                case RoomUpdateType.Updated:
                    break;
                //return SendUpdateEmail(room, user.Id, "Room has been updated.");
                case RoomUpdateType.Entered:
                    return SendUpdateEmail(room, $"<b>{user.Fullname} entered the room.</b>", others(room, user.Id));
                case RoomUpdateType.Left:
                    return SendUpdateEmail(room, $"<b>{user.Fullname} left the room.</b>", others(room, user.Id));
                case RoomUpdateType.Removed:
                    return SendUpdateEmail(room, $"<b>{user.Fullname} deleted the room you have previously entered.</b> You can go ahead and create your own room for that time.", others(room, user.Id));
                case RoomUpdateType.Joined:
                    break;
            }

            return Task.CompletedTask;
        }

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
                        Subject = "Room update",
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
