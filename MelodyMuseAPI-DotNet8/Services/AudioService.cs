using MongoDB.Bson;

namespace MelodyMuseAPI_DotNet8.Services
{
    public class AudioService
    {
        private readonly MongoDbService _mongoDbService;

        public AudioService(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        public async Task<ObjectId> UploadAudioAsync(Stream fileStream, string fileName)
        {
            return await _mongoDbService.UploadFileToGridFSAsync(fileStream, fileName);
        }

        public async Task<Stream> DownloadAudioAsync(ObjectId id)
        {
            return await _mongoDbService.DownloadFileFromGridFSAsync(id);
        }

        public async Task DeleteAudioAsync(ObjectId id)
        {
            await _mongoDbService.DeleteFileFromGridFSAsync(id);
        }
    }

}
