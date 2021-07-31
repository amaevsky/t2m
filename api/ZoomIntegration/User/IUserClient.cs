using System.Threading.Tasks;

namespace Lingua.ZoomIntegration
{
    public interface IUserClient
    {
        Task<ZoomClientResponse<UserProfile>> GetUserProfile(AccessTokens accessToken);
    }
}
