using System.Threading.Tasks;

namespace Lingua.ZoomIntegration
{
    public interface IAuthService
    {
        Task<AccessTokenResponse> RequestAccessToken(string authCode);
        Task<AccessTokenResponse> RefreshAccessToken(string refreshToken);
    }
}
