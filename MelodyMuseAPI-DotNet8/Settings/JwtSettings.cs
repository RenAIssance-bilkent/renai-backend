namespace MelodyMuseAPI_DotNet8.Settings
{
    public class JwtSettings
    {
        public string SecretKey { get; set; } = null!;
        public int ExpirationMinutes { get; set; } = 0;
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
