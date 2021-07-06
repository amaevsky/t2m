using System;

namespace Lingua.ZoomIntegration
{
    public class AccessTokens
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
