namespace MelodyMuseAPI.Settings
{
    public class AuthMessageSenderSettings
    {
        public string? SendGridKey { get; set; }
        public string? BaseURL { get; set; }
        public string? WelcomeTemplateId { get; set; }
        public string? ConfirmTemplateId { get; set; }
        public string? ResetTemplateId { get; set; }
    }
}
