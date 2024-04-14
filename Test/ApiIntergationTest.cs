using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MelodyMuseAPI.Settings;

namespace Tests
{
    public class ApiIntergationTest
    {
        private OpenAIApiService _service;

        [SetUp]
        public void Setup()
        {
            var httpClient = new HttpClient();
            var settings = new OpenAISettings
            {
                ApiEndpoint = "https://api.openai.com/v1",
                ApiKey = "",
                SystemPrompt = "Act as a transformer, which takes text as input and returns a JSON object as output. The text that you will get will be a description of songs that the user wants to generate, your responsibility is to return a genre that is relevant to that description from the following selection: ELECTRONIC, AMBIENT, HIP-HOP, POP, ORCHESTRAL. Example: User: \"I want some energetic music for exercise\"  Assistant: {\"genre\": \"ELECTRONIC\"}",
            };

            IOptions<OpenAISettings> options = Options.Create(settings);
            _service = new OpenAIApiService(httpClient, options);
        }

        [Test]
        public async Task GetMetadataFromPrompt_ReturnsCorrectResponse()
        {
            var result = await _service.GetMetadataFromPrompt("I want some energetic music for exercise", "1");
            Assert.That(result, Is.EqualTo("{\"genre\": \"ELECTRONIC\"}"));
        }
    }
}
