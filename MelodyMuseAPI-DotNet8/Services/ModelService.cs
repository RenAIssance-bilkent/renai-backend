using MelodyMuseAPI.Dtos;
using System.IO;
using System.Threading.Tasks;

namespace MelodyMuseAPI.Services
{
    public class ModelService
    {
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
}
