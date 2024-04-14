using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text;
using MelodyMuseAPI.Settings;
using MelodyMuseAPI.Models;

public class OpenAIApiService
{
    private readonly HttpClient _httpClient;
    private readonly OpenAISettings _openAiSettings;

    public OpenAIApiService(HttpClient httpClient, IOptions<OpenAISettings> openAiSettings)
    {
        _httpClient = httpClient;
        _openAiSettings = openAiSettings.Value;
    }

    public async Task<string> GetMetadataFromPrompt(string prompt, string userId)
    {
        var requestData = new
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
                new { role = "system", content = _openAiSettings.SystemPrompt },
                new { role = "user", content = prompt }
            },
            user = userId
        };

        var json = JsonSerializer.Serialize(requestData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _openAiSettings.ApiKey);

        var response = await _httpClient.PostAsync(_openAiSettings.ApiEndpoint + "/chat/completions", content);

        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<OpenAIResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (responseData == null || responseData.Choices == null || responseData.Choices.Length == 0 || responseData.Choices[0].Message == null)
            {
                throw new InvalidOperationException("Received an unexpected format of response data.");
            }

            return responseData.Choices[0].Message.Content.Trim();
        }
        else
        {
            throw new HttpRequestException($"Failed to retrieve data from OpenAI: {response.StatusCode}");
        }

    }

    public class OpenAIResponse
    { 
        public Choice[] Choices { get; set; }
    }

    public class Choice
    {
        public Message Message { get; set; }
    }

    public class Message
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }

}
