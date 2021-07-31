using System.Threading.Tasks;

namespace Lingua.ZoomIntegration
{
    public interface IMeetingClient
    {
        Task<ZoomClientResponse<Meeting>> CreateMeeting(AccessTokens accessToken, CreateMeetingRequest request);
    }
}
