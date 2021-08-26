using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Lingua.ZoomIntegration
{
    public abstract class BaseClient
    {

        private readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            },
            NullValueHandling = NullValueHandling.Ignore
        };

        public T Deserialize<T>(string obj)
        {
            return JsonConvert.DeserializeObject<T>(obj, SerializerSettings);
        }

        public string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, SerializerSettings);
        }

        public async Task<T> ExtractResponse<T>(HttpResponseMessage response)
        {
            var str = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return Deserialize<T>(str);
            }

            throw new ZoomClientException(str, "Zoom client request failed.");
        }

        public async Task UseToken(IAuthClient authClient, AccessTokens tokens, Func<string, AccessTokens, Task> action)
        {
            if (tokens.ExpiresAt > DateTime.UtcNow)
            {
                await action(tokens.AccessToken, null);
            }
            else
            {
                var newTokens = await authClient.RefreshAccessToken(tokens.RefreshToken);
                tokens.AccessToken = newTokens.AccessToken;
                tokens.RefreshToken = newTokens.RefreshToken;
                tokens.ExpiresAt = newTokens.ExpiresAt;

                await action(tokens.AccessToken, tokens);
            }
        }
    }
}