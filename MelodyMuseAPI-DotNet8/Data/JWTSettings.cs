namespace MelodyMuseAPI_DotNet8.Data
{
    public class JWTSettings
    {
        public string SecretKey { get; set; } = null!;
        public int ExpirationMinutes { get; set; } = 0;
    }
}
