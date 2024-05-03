using Microsoft.VisualStudio.TestTools.UnitTesting;
using MelodyMuseAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelodyMuseAPI.Dtos;
using MelodyMuseAPI.Models;
using Moq;

namespace MelodyMuseAPI.Services.Tests
{
    [TestClass]
    public class TrackServiceTests
    {
        private Mock<MongoDbService> _mockMongoDbService;
        private Mock<OpenAIApiService> _mockOpenAIApiService;
        private Mock<ModelService> _mockModelService;
        private TrackService _trackService;

        [TestInitialize]
        public void Initialize()
        {
            _mockMongoDbService = new Mock<MongoDbService>();
            _mockOpenAIApiService = new Mock<OpenAIApiService>();
            _mockModelService = new Mock<ModelService>();
            _trackService = new TrackService(_mockMongoDbService.Object, _mockOpenAIApiService.Object, _mockModelService.Object);
        }

        [TestMethod]
        public async Task GetAllTracksTest_ReturnsTracks()
        {
            var tracks = new List<Track> { new Track { Title = "Track 1" } };
            _mockMongoDbService.Setup(x => x.GetAllTracksAsync()).ReturnsAsync(tracks);

            var result = await _trackService.GetAllTracks();

            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public async Task DeleteTrackTest_TrackDeleted()
        {
            var trackId = "track123";
            _mockMongoDbService.Setup(x => x.DeleteTrackAsync(trackId)).ReturnsAsync(true);

            var result = await _trackService.DeleteTrack(trackId);
            Assert.IsTrue(result);
        }
    }
}