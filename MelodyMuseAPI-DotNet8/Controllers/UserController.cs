using MelodyMuseAPI.Models;
using MelodyMuseAPI_DotNet8.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MelodyMuseAPI_DotNet8.Controllers
{
    [Controller]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly MongoDBService _mongoDBService;
        public UserController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        #region Testing

        [HttpGet]
        public async Task<List<User>> Get()
        {
            return await _mongoDBService.GetAllAsync();
        }

        #endregion
    }
}