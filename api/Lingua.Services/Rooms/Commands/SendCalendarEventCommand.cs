using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Lingua.Shared;
using MediatR;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Lingua.Services.Rooms.Commands
{
    public class SendCalendarEventCommand : BaseRoomCommand
    {

    }

    public class SendCalendarEventCommandHandler : IRequestHandler<SendCalendarEventCommand>
    {
        private readonly IRoomRepository _roomRepository;
        private readonly ITemplateProvider _templateProvider;
        private readonly IEmailService _emailService;

        public SendCalendarEventCommandHandler(IRoomRepository roomRepository, ITemplateProvider templateProvider, IEmailService emailService)
        {
            _roomRepository = roomRepository;
            _templateProvider = templateProvider;
            _emailService = emailService;
        }

        public async Task<Unit> Handle(SendCalendarEventCommand command, CancellationToken cancellationToken)
        {
            var room = await _roomRepository.Get(command.RoomId);
            var user = room.Participants.Find(p => p.Id == command.UserId);

            var body = await _templateProvider.GetCalendarEventEmail(user);
            var message = new EmailMessage
            {
                Subject = "Calendar Event",
                Body = body,
                IsHtml = true
            };
            message.Attachments.Add(CreateICSAttachment(room));

            await _emailService.SendAsync(message, user.Email);

            return Unit.Value;
        }

        private System.Net.Mail.Attachment CreateICSAttachment(Room room, bool isCancel = false)
        {
            var e = new CalendarEvent
            {
                Summary = "Talk2Me Room",
                Start = new CalDateTime(room.StartDate),
                End = new CalDateTime(room.EndDate),
                Uid = room.Id.ToString()
            };

            var calendar = new Calendar();
            calendar.Method = "PUBLISH";
            calendar.Events.Add(e);

            if (isCancel)
            {
                calendar.Method = "CANCEL";
                e.Status = "CANCELLED";
                e.Sequence = 1;
            }

            var serializer = new CalendarSerializer();
            var serializedCalendar = serializer.SerializeToString(calendar);
            var bytes = System.Text.Encoding.UTF8.GetBytes(serializedCalendar);
            MemoryStream stream = new MemoryStream(bytes);

            return new System.Net.Mail.Attachment(stream, "room.ics");
        }
    }
}
