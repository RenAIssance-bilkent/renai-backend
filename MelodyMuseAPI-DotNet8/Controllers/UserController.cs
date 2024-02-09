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
            
        }
    }
}