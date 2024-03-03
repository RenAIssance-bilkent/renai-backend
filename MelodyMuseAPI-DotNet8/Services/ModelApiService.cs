using MelodyMuseAPI.Dtos;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Options;
using MelodyMuseAPI.Settings;

namespace MelodyMuseAPI.Services
{
    public class ModelApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _modelApiBaseUrl;

        public ModelApiService(HttpClient httpClient, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClient;
            _modelApiBaseUrl = apiSettings.Value.ModelApiBaseUrl;
        }

        public async Task<string> GenerateTrackAsync(TrackModelGenerationDto trackModelGenerationDto)
        {
            var json = JsonSerializer.Serialize(trackModelGenerationDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Console.WriteLine(content);

            var response = await _httpClient.PostAsync($"{_modelApiBaseUrl}/generate", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to generate track");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var generatedTrackResponse = JsonSerializer.Deserialize<TrackModelGenerationResponse>(responseBody);

            //TODO: add exception handeling

            return generatedTrackResponse?.TrackId;
        }
    }
}
