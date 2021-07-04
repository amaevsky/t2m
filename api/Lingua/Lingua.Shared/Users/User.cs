namespace Lingua.Shared
{
    public class User: AuditableEntity
    {
        public string Lastname { get; set; }
        public string  Firstname { get; set; }
        public string Email { get; set; }
        public string TargetLanguage { get; set; }
        public ZoomProperties ZoomProperties { get; set; }
    }

    public class ZoomProperties
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
