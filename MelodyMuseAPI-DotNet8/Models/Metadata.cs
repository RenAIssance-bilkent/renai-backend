using System.Text.Json.Serialization;

namespace MelodyMuseAPI.Models
{
    public class Metadata
    {
        [JsonPropertyName("model")]
        public int Model { get; set; }

        [JsonPropertyName("genre")]
        public string Genre { get; set; }

        [JsonPropertyName("title")] // Assuming you might receive a title in JSON.
        public string Title { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("enhanced_prompt")]
        public string EnhancedPrompt { get; set; }

        [JsonPropertyName("danceability")]
        public double Danceability { get; set; }

        [JsonPropertyName("energy")]
        public double Energy { get; set; }

        [JsonPropertyName("loudness")]
        public double Loudness { get; set; }

        [JsonPropertyName("valence")]
        public double Valence { get; set; }

    }
}
