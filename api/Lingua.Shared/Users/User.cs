using Lingua.ZoomIntegration;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;

namespace Lingua.Shared
{
    [BsonIgnoreExtraElements]
    public class User : AuditableEntity
    {
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string TargetLanguage { get; set; }
        public string LanguageLevel { get; set; }
        public string AvatarUrl { get; set; }
        public string Timezone { get; set; }
        public ZoomProperties ZoomProperties { get; set; }

        public string Fullname => $"{Firstname} {Lastname}";
        public bool IsReady => !string.IsNullOrEmpty(LanguageLevel);
    }

    public class ZoomProperties
    {
        public string UserId { get; set; }
        public AccessTokens AccessTokens { get; set; }
    }
}
