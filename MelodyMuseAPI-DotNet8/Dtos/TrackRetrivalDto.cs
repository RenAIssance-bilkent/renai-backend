using MelodyMuseAPI.Models;

namespace MelodyMuseAPI.Dtos
{
    public class TrackRetrivalDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Metadata Metadata { get; set; }
        public int Model { get; set; }
        public string ImageEndpoint { get; set; }
        public string AudioEndpoint { get; set; } 
    }
}
