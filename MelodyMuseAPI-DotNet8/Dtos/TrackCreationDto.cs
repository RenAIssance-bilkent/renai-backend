namespace MelodyMuseAPI.Dtos
{
    public class TrackCreationDto
    {
        public string Genre { get; set; }
        public string Prompt { get; set; }
        public int Model { get; set; }
        public double? Danceability {  get; set; }
        public double? Energy { get; set; }
        public double? Loudness { get; set; }
        public double? Valence { get; set; }

    }

}
