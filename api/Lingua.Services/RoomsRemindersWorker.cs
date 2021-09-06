using Lingua.Shared;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lingua.Services
{
    public class RoomsRemindersWorker : BackgroundService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IEmailService _emailService;
        private readonly ITemplateProvider _templateProvider;
        private readonly IDateTimeProvider _dateTime;
        private readonly ILogger<RoomsRemindersWorker> _logger;
        private const int REMINDER_INTERVAL_MIN = 60;
        private const int CYCLE_INTERVAL_SEC = 60;
        private DateTime? _lastEndDate = null;


        public RoomsRemindersWorker(IRoomRepository roomRepository,
                                    IEmailService emailService,
                                    ITemplateProvider templateProvider,
                                    IDateTimeProvider dateTime,
                                    ILogger<RoomsRemindersWorker> logger)
        {
            _roomRepository = roomRepository;
            _emailService = emailService;
            _templateProvider = templateProvider;
            _dateTime = dateTime;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await DoCycle();
                    await Task.Delay(CYCLE_INTERVAL_SEC * 1000);
                }
            });
        }

        public async Task DoCycle()
        {
            try
            {
                var from = _lastEndDate ?? _dateTime.UtcNow.AddMinutes(REMINDER_INTERVAL_MIN).AddSeconds(CYCLE_INTERVAL_SEC * -1);
                var to = _dateTime.UtcNow.AddMinutes(REMINDER_INTERVAL_MIN);

                var rooms = (await _roomRepository.Get(r => r.StartDate > from
                                                        && r.StartDate <= to))
                                                .Where(r => r.IsFull);

                foreach (var room in rooms)
                {
                    SendUpdateEmail(room, room.Participants.ToArray()).ConfigureAwait(false);
                }

                _lastEndDate = to;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Reminder worker cycle failed to complete.");
            }
        }

        private async Task SendUpdateEmail(Room room, params User[] recipients)
        {
            foreach (var recipient in recipients)
            {
                try
                {
                    var body = await _templateProvider.GetRoomReminderEmail(room, recipient);
                    var email = new EmailMessage
                    {
                        Subject = "Your Room Starts in 1 Hour",
                        Body = body,
                        IsHtml = true,
                    };

                    await _emailService.SendAsync(email, recipient.Email).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, $"Room:{room?.Id} reminder message is failed to sent to {recipient?.Email}");
                }
            }
        }
    }
}
