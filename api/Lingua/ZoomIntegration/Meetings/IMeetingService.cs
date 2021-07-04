using System.Threading.Tasks;

namespace Lingua.ZoomIntegration
{
    public interface IMeetingService
    {
        Task<Meeting> CreateMeeting(string accessToken, CreateMeetingRequest request);
    }
}
