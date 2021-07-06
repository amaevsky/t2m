using System;
using System.Threading.Tasks;

namespace Lingua.ZoomIntegration.Auth
{
    public interface ITokenProvider
    {
        Task<bool> UseToken(AccessTokens tokens, Func<AccessTokens, Task> action);
    }
}
