namespace Lingua.API
{
    public class JwtOptions
    {
        public string Issuer { get; set; }
        public string EncryptionKey { get; set; }
        public int? Expiration { get; set; }
    }
}