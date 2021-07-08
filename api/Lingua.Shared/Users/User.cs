using Lingua.ZoomIntegration;

namespace Lingua.Shared
{
    public class User : AuditableEntity
    {
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string Email { get; set; }
        public string TargetLanguage { get; set; }
        public string LanguageLevel { get; set; }
        public string AvatarUrl { get; set; }
        public string Timezone { get; set; }
        public ZoomProperties ZoomProperties { get; set; }
    }

    public class ZoomProperties
    {
        public AccessTokens AccessTokens { get; set; }
    }
}
