using Lingua.Shared;
using System.Threading.Tasks;

namespace Lingua.Shared
{
    public interface ITemplateProvider
    {
        Task<string> GetRoomUpdateEmail(string message, Room room, User recepient);
        Task<string> GetWelcomeLetterEmail(User recepient);
    }
}
