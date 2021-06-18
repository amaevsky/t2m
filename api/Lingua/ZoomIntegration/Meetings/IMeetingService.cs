using System.Threading.Tasks;

namespace Lingua.ZoomIntegration
{
    public interface IMeetingService
    {
        Task<Meeting> CreateMeeting(string accessToken, string userId, CreateMeetingRequest request);
    }
}
