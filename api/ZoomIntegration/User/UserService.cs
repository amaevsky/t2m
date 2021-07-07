using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Lingua.ZoomIntegration
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        public UserService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.zoom.us/v2/users/me");
        }

        public async Task<UserProfile> GetUserProfile(string accessToken)
        {
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.Headers.Add("Authorization", $"Bearer {accessToken}");

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                dynamic resp = JsonConvert.DeserializeObject<object>(await response.Content.ReadAsStringAsync());
                return new UserProfile
                {
                    Id = resp.id,
                    Firstname = resp.first_name,
                    Lastname = resp.last_name,
                    Email = resp.email,
                    PicUrl = resp.pic_url
                };
            }

            throw new Exception("Cannot get user profile");
        }
    }
}
