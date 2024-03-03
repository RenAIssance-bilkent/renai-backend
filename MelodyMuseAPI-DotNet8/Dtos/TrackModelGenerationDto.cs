namespace MelodyMuseAPI.Dtos
{
    public class TrackModelGenerationDto
    {
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Metadata { get; set; }

    }
}
