namespace MelodyMuseAPI.Settings
{
    public class OpenAISettings
    {
        public string ApiEndpoint { get; set; }
        public string ApiKey { get; set; }
        public string SystemPrompt { get; set; }
        public string GenreSystemPrompt { get; set; }
        public string ImgPrompt { get; set; }
    }
}
