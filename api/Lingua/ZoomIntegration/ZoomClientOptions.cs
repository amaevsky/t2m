namespace Lingua.ZoomIntegration
{
    public class ZoomClientOptions
    {
        public ZoomClientOptions(string clientId, string clientSecret, string redirectUri)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            RedirectUri = redirectUri;

        }

        public string ClientId { get; }
        public string ClientSecret { get; }
        public string RedirectUri { get; }
    }
}
