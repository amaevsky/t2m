using Lingua.Shared;
using System.Threading.Tasks;

namespace Lingua.EmailTemplates
{
    public class TemplateProvider : ITemplateProvider
    {
        private readonly IViewRenderService _viewRenderService;

        public TemplateProvider(IViewRenderService viewRenderService)
        {
            _viewRenderService = viewRenderService;
        }

        public Task<string> GetCalendarEventEmail(User recipient)
        {
            return _viewRenderService.RenderToStringAsync("CalendarEvent",
                new BaseModel
                {
                    Recepient = recipient
                });
        }

        public Task<string> GetRoomReminderEmail(Room room, User recipient)
        {
            return _viewRenderService.RenderToStringAsync("RoomReminder",
                new RoomModel
                {
                    Room = room,
                    Recepient = recipient
                });
        }

        public Task<string> GetRoomUpdateEmail(string message, Room room, User recipient)
        {
            return _viewRenderService.RenderToStringAsync("RoomUpdate",
                new RoomUpdateModel
                {
                    Message = message,
                    Room = room,
                    Recepient = recipient
                });
        }

        public Task<string> GetWelcomeLetterEmail(User recipient)
        {
            return _viewRenderService.RenderToStringAsync("WelcomeLetter",
                new BaseModel
                {
                    Recepient = recipient
                });
        }
    }
}
