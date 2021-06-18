﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lingua.ZoomIntegration
{
    public class AuthService : IAuthService
    {
        private readonly ZoomClientOptions _options;
        private readonly HttpClient _httpClient;

        public AuthService(ZoomClientOptions options)
        {
            _options = options;

            _httpClient = new HttpClient();
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_options.ClientId}:{_options.ClientSecret}"));
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {encoded}");
            _httpClient.BaseAddress = new Uri("https://zoom.us/oauth/token");

        }

        public async Task<AccessTokenResponse> RefreshAccessToken(string refreshToken)
        {
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                        { "refresh_token", refreshToken },
                        { "grant_type", "refresh_token" }
                }
            );

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                dynamic resp = JsonConvert.DeserializeObject<object>(await response.Content.ReadAsStringAsync());

                return new AccessTokenResponse
                {
                    AccessToken = resp.access_token,
                    RefreshToken = resp.refresh_token
                };
            }

            throw new Exception("Cannot refresh access token");
        }

        public async Task<AccessTokenResponse> RequestAccessToken(string authCode)
        {
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                        { "code", authCode },
                        { "grant_type", "authorization_code" },
                        { "redirect_uri", _options.RedirectUri }
                }
            );

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                dynamic resp = JsonConvert.DeserializeObject<object>(await response.Content.ReadAsStringAsync());

                return new AccessTokenResponse
                {
                    AccessToken = resp.access_token,
                    RefreshToken = resp.refresh_token
                };
            }

            throw new Exception("Cannot get access token");
        }

    }
}
