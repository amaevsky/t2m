using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lingua.ZoomIntegration
{
    public class AuthClient : BaseClient, IAuthClient
    {
        private readonly ZoomClientOptions _options;
        private readonly HttpClient _httpClient;

        public AuthClient(IOptions<ZoomClientOptions> options)
        {
            _options = options.Value;

            _httpClient = new HttpClient();
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_options.ClientId}:{_options.ClientSecret}"));
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {encoded}");
            _httpClient.BaseAddress = new Uri("https://zoom.us/oauth/token");

        }

        public async Task<AccessTokens> RefreshAccessToken(string refreshToken)
        {
            var body = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                        { "refresh_token", refreshToken },
                        { "grant_type", "refresh_token" }
                }
            );

            return await RequestAccessToken(body);
        }

        public async Task<AccessTokens> RequestAccessToken(string authCode)
        {
            var body = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                        { "code", authCode },
                        { "grant_type", "authorization_code" },
                        { "redirect_uri", _options.RedirectUri }
                }
            );

            return await RequestAccessToken(body);
        }

        public async Task<AccessTokens> RequestAccessToken(HttpContent body)
        {
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.Content = body;

            var response = await _httpClient.SendAsync(request);
            var resp = await ExtractResponse<AccessTokenResponse>(response);

            return new AccessTokens
            {
                AccessToken = resp.AccessToken,
                RefreshToken = resp.RefreshToken,
                ExpiresAt = DateTime.UtcNow.AddSeconds(resp.ExpiresIn)
            };
        }

    }
}
