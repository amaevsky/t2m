using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Lingua.ZoomIntegration
{
    public class UserClient : BaseClient, IUserClient
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthClient _authClient;

        public UserClient(IAuthClient authClient)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.zoom.us/v2/users/me");
            _authClient = authClient;
        }

        public async Task<ZoomClientResponse<UserProfile>> GetUserProfile(AccessTokens accessToken)
        {
            ZoomClientResponse<UserProfile> finalResponse = null;
            await UseToken(_authClient, accessToken, async (accessToken, newTokens) =>
             {
                 var request = new HttpRequestMessage();
                 request.Method = HttpMethod.Get;
                 request.Headers.Add("Authorization", $"Bearer {accessToken}");

                 var response = await _httpClient.SendAsync(request);
                 var userProfile = await ExtractResponse<UserProfile>(response);
                 finalResponse = new ZoomClientResponse<UserProfile>(userProfile, newTokens);
             });

            return finalResponse;
        }
    }
}
