using System;
using System.Threading.Tasks;

namespace Lingua.ZoomIntegration.Auth
{
    public class RefreshableTokenProvider : ITokenProvider
    {
        private readonly IAuthClient _authClient;

        public RefreshableTokenProvider(IAuthClient authClient)
        {
            _authClient = authClient;
        }

        public async Task<bool> UseToken(AccessTokens tokens, Func<AccessTokens, Task> action)
        {
            if (tokens.ExpiresAt > DateTime.UtcNow)
            {
                await action(tokens);
                return false;
            }

            var newTokens = await _authClient.RefreshAccessToken(tokens.RefreshToken);
            tokens.AccessToken = newTokens.AccessToken;
            tokens.RefreshToken = newTokens.RefreshToken;
            tokens.ExpiresAt = newTokens.ExpiresAt;

            return true;
        }
    }
}
