using MelodyMuseAPI.Models;

namespace MelodyMuseAPI.Dtos
{
    public class TrackGenerationDto
    {
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public DateTime CreatedAt { get; set; }
        public Metadata Metadata { get; set; }
        public int Model { get; set; }

    }
}
