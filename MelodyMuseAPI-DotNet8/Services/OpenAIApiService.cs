namespace MelodyMuseAPI.Services
{
    public class OpenAIApiService
    {
        private readonly HttpClient _httpClient;

        public OpenAIApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Can be changed to numerical values for optimization
        public String GetMetadataFromPrompt(string prompt)
        {
            // TODO: Connect to OpenAI API
            return "test";
        }
    }
}
