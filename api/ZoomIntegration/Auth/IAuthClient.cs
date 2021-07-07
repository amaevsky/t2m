using System.Threading.Tasks;

namespace Lingua.ZoomIntegration
{
    public interface IAuthClient
    {
        Task<AccessTokens> RequestAccessToken(string authCode);
        Task<AccessTokens> RefreshAccessToken(string refreshToken);
    }
}
