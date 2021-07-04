using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lingua.ZoomIntegration
{
    public class MeetingService : IMeetingService
    {
        private readonly HttpClient _httpClient;
        public MeetingService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.zoom.us/v2/users/{userId}/meetings");
        }

        public async Task<Meeting> CreateMeeting(string accessToken, CreateMeetingRequest request)
        {
            var message = new HttpRequestMessage();
            message.Method = HttpMethod.Post;
            message.Headers.Add("Authorization", $"Bearer {accessToken}");
            message.RequestUri = new Uri("https://api.zoom.us/v2/users/me/meetings");

            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                },
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(request, serializerSettings);
            message.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(message);
            if (response.IsSuccessStatusCode)
            {
                var meeting = JsonConvert.DeserializeObject<Meeting>(await response.Content.ReadAsStringAsync(), serializerSettings);
                return meeting;
            }
            else
            {
                var str = await response.Content.ReadAsStringAsync();
                dynamic a = JsonConvert.DeserializeObject<object>(str);
            }

            throw new Exception("Cannot get user profile");
        }
    }
}
