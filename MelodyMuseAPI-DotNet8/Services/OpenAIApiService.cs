﻿using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text;
using MelodyMuseAPI.Settings;
using MelodyMuseAPI.Models;
using MelodyMuseAPI.Controllers;
using MelodyMuseAPI.Dtos;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

public class OpenAIApiService
{
    private readonly HttpClient _httpClient;
    private readonly OpenAISettings _openAiSettings;

    public OpenAIApiService(HttpClient httpClient, IOptions<OpenAISettings> openAiSettings)
    {
        _httpClient = httpClient;
        _openAiSettings = openAiSettings.Value;
    }
    private static readonly List<string> _genres = new List<string>
    {
            "ORCHESTRAL",
            "POP",
            "ROCK",
            "ELECTRONIC",
    };
    public async Task<string> GenerateRandomPrompt(string userId)
    {
        var randomIndex = new Random().Next(_genres.Count);
        return await GeneratePromptBasedOnGenre(userId, _genres[randomIndex]);
    }
    public async Task<string> GeneratePromptBasedOnGenre(string genre, string userId)
    {
        var requestData = new
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
                new { role = "system", content = _openAiSettings.GenreSystemPrompt},
                new { role = "user", content = genre }
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
    public async Task<Metadata> GetMetadataFromPromptForReplica(TrackCreationDto trackCreationDto, string userId)
    {
        var prompt = trackCreationDto.Prompt;
        if (trackCreationDto.Energy != null && trackCreationDto.Danceability != null &&
            trackCreationDto.Loudness != null && trackCreationDto.Valence != null)
        {
            prompt += $", danceability: {trackCreationDto.Danceability:F1}, energy: {trackCreationDto.Energy:F1}, " +
                      $"loudness: {trackCreationDto.Loudness:F1}, valence: {trackCreationDto.Valence:F1}";
        }

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

            var metadata = JsonSerializer.Deserialize<Metadata>(responseData.Choices[0].Message.Content.Trim(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (metadata == null)
            {
                throw new InvalidOperationException("Failed to deserialize the metadata content.");
            }

            metadata.Model = trackCreationDto.Model;

            return metadata;
        }
        else
        {
            throw new HttpRequestException($"Failed to retrieve data from OpenAI: {response.StatusCode}");
        }
    }
    public async Task<Stream> GenerateImageFileFromPrompt(string prompt, string userId)
    {
        var combinedPrompt = _openAiSettings.ImgPrompt + prompt;

        var requestData = new
        {
            model = "dall-e-3", 
            prompt = combinedPrompt,
            n = 1,
            size = "1024x1024",
            user = userId
        };

        var json = JsonSerializer.Serialize(requestData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _openAiSettings.ApiKey);

        var response = await _httpClient.PostAsync(_openAiSettings.ApiEndpoint + "/images/generations", content);

        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            using (var doc = JsonDocument.Parse(jsonResponse))
            {
                var url = doc.RootElement
                            .GetProperty("data")
                            .EnumerateArray()
                            .First()
                            .GetProperty("url")
                            .GetString();

                if (string.IsNullOrEmpty(url))
                {
                    throw new InvalidOperationException("Image URL is missing in the response.");
                }

                using (var client = new HttpClient())
                {
                    var imageResponse = await client.GetAsync(url);
                    if (!imageResponse.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException($"Failed to download image: {imageResponse.StatusCode}");
                    }

                    using (var imageStream = await imageResponse.Content.ReadAsStreamAsync())
                    {
                        using (var image = Image.Load(imageStream))
                        {
                            // Resize the image
                            image.Mutate(x => x.Resize(256, 256));

                            // Convert image to a Stream to return
                            var outputStream = new MemoryStream();
                            image.Save(outputStream, new PngEncoder());
                            outputStream.Position = 0;
                            return outputStream;
                        }
                    }
                }
            }
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error response content: {errorContent}");
            throw new HttpRequestException($"Failed to retrieve image from OpenAI: {response.StatusCode}, Content: {errorContent}");
        }
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
