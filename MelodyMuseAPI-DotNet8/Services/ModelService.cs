using MelodyMuseAPI.Dtos;
using System.Text.Json;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Options;
using MelodyMuseAPI.Settings;
using RenaiML;

namespace MelodyMuseAPI.Services
{
    public class ModelService
    {
        // This code makes no sense
        public async Task<string> GenerateTrackAsync(TrackModelGenerationDto trackModelGenerationDto) {

            var sampleData = new SemanticModel.ModelInput()
            {
                Col0 = trackModelGenerationDto.Metadata,
            };

            var result = SemanticModel.Predict(sampleData);

            return result.ToString();
        }
    }
}
