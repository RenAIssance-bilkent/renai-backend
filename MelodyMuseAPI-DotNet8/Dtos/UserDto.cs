namespace MelodyMuseAPI.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public int Points { get; set; }
        public List<string> TrackIds { get; set; } = new List<string>();
    }
}
