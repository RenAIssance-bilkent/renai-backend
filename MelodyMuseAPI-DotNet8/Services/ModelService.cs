using MelodyMuseAPI.Dtos;
using MelodyMuseAPI.Settings;
using MelodyMuseAPI.Utils;
using Microsoft.Extensions.Options;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace MelodyMuseAPI.Services
{
    public class ModelService
    {
        private readonly HttpClient _httpClient;
        private readonly ReplicateSettings _replicateSettings;

        public ModelService(HttpClient httpClient, IOptions<ReplicateSettings> replicateSettings)
        {
            _httpClient = httpClient;
            _replicateSettings = replicateSettings.Value;
        }

        public async Task<Stream> GenerateWithReplicateAsync(TrackGenerationDto trackModelGenerationDto)
        {
            var metadata = new JsonMetadata(trackModelGenerationDto.Metadata);

            var requestBody = new
            {
                version = _replicateSettings.Version,
                input = new { prompt = metadata.GetValue("enhanced_prompt") }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _replicateSettings.ApiToken);

            // Initial POST request to start the generation
            var response = await _httpClient.PostAsync(_replicateSettings.ApiEndpoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to start generation: {response.StatusCode}, Content: {errorContent}");
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var predictionData = JsonSerializer.Deserialize<ReplicatePrediction>(await response.Content.ReadAsStringAsync(), options);

            var predictionUrl = predictionData.Urls.Get;

            // Poll for result
            while (true)
            {
                await Task.Delay(2000); // Wait for 2 seconds before polling again
                var pollResponse = await _httpClient.GetAsync(predictionUrl);
                var pollData = JsonSerializer.Deserialize<ReplicatePrediction>(await pollResponse.Content.ReadAsStringAsync(), options);

                if (pollData.Status == "succeeded")
                {
                    var outputFileResponse = await _httpClient.GetAsync(pollData.Output);
                    if (outputFileResponse.IsSuccessStatusCode)
                    {
                        return await outputFileResponse.Content.ReadAsStreamAsync();
                    }
                    throw new Exception("Failed to download the output file.");
                }
                else if (pollData.Status == "failed")
                {
                    throw new Exception("Prediction failed.");
                }
            }
        }

        public async Task<Stream> GenerateAudioAsync(TrackGenerationDto trackModelGenerationDto)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "test.wav");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified audio file could not be found.", filePath);
            }
            return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
        }
    }

    // For JSON Serialization, I am sure there is more sophisticated way but this is good enough
    public class ReplicatePrediction
    {
        public string Id { get; set; }
        public string Model { get; set; }
        public string Version { get; set; }
        public Input Input { get; set; }
        public string Output { get; set; }
        public string Logs { get; set; }
        public string Error { get; set; }
        public string Status { get; set; }
        public string CreatedAt { get; set; }
        public Urls Urls { get; set; }
    }

    public class Input
    {
        public string Prompt { get; set; }
    }

    public class Urls
    {
        public string Cancel { get; set; }
        public string Get { get; set; }
    }

}
