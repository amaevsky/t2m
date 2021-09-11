using Lingua.Shared;
using System.Threading.Tasks;

namespace Lingua.Shared
{
    public interface ITemplateProvider
    {
        Task<string> GetRoomUpdateEmail(string message, Room room, User recipient);
        Task<string> GetRoomReminderEmail(Room room, User recipient);
        Task<string> GetWelcomeLetterEmail(User recipient);
        Task<string> GetCalendarEventEmail(User recipient);
    }
}
