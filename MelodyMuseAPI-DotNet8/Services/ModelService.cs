using MelodyMuseAPI.Dtos;
using System.Text.Json;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Options;
using MelodyMuseAPI.Settings;

namespace MelodyMuseAPI.Services
{
    public class ModelService
    {
        public async Task<string> GenerateTrackAsync(TrackModelGenerationDto trackModelGenerationDto) { 
        
            return trackModelGenerationDto.Metadata;
        }
    }
}
