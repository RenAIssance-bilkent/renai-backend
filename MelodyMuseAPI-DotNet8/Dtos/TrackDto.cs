using MongoDB.Bson;

namespace MelodyMuseAPI.Dtos
{
    public class TrackDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Metadata { get; set; }
        public string Model { get; set; }
        public ObjectId ImageId { get; set; }
        public ObjectId AudioId { get; set; }
        public string ImageBase64 { get; set; }
        public string AudioBase64 { get; set; }
    }
}
