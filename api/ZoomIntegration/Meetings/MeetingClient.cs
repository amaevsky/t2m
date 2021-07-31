using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lingua.ZoomIntegration
{
    public class MeetingClient : BaseClient, IMeetingClient
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthClient _authClient;

        public MeetingClient(IAuthClient authClient)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.zoom.us/v2/users/{userId}/meetings");
            _authClient = authClient;
        }

        public async Task<ZoomClientResponse<Meeting>> CreateMeeting(AccessTokens accessTokens, CreateMeetingRequest request)
        {
            ZoomClientResponse<Meeting> finalResponse = null;

            await UseToken(_authClient, accessTokens, async (accessToken, newTokens) =>
           {
               var message = new HttpRequestMessage();
               message.Method = HttpMethod.Post;
               message.Headers.Add("Authorization", $"Bearer {accessToken}");
               message.RequestUri = new Uri("https://api.zoom.us/v2/users/me/meetings");

               var json = Serialize(request);
               message.Content = new StringContent(json, Encoding.UTF8, "application/json");

               var response = await _httpClient.SendAsync(message);
               var meeting = await ExtractResponse<Meeting>(response);
               finalResponse = new ZoomClientResponse<Meeting>(meeting, newTokens);
           });

            return finalResponse;
        }
    }
}
