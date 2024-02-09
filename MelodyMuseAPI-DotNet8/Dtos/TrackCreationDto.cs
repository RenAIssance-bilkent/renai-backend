﻿namespace MelodyMuseAPI_DotNet8.Dtos
{
    public class TrackCreationDto
    {
        public string Title { get; set; }
        public string Genre { get; set; }
        public string AudioURL { get; set; }
        public string Metadata { get; set; }

        // for example, file uploads for the track itself if not handled by the AudioURL.
    }

}
