using NUnit.Framework;
using Moq;
using MelodyMuseAPI.Models;
using MongoDB.Driver;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MelodyMuseAPI_DotNet8.Dtos;
using MelodyMuseAPI_DotNet8.Services;
using MelodyMuseAPI_DotNet8.Interfaces;
using System.Xml.Linq;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using MelodyMuseAPI_DotNet8.Settings;

[TestFixture]
public class UserUnitTest
{
    private UserService _userService;

    [SetUp]
    public void Setup()
    {
        MongoDbSettings settings = new MongoDbSettings() { 
            ConnectionURI = "mongodb+srv://cluster0.xkhwfdr.mongodb.net/?authSource=%24external&authMechanism=MONGODB-X509&retryWrites=true&w=majority",
            DatabaseName = "melodymuse",
            CertificatePath = "X509-cert-5005428808408647310.pfx"
        };
        IOptions<MongoDbSettings> myOptions = Options.Create(settings);

        var mongoDbService = new MongoDbService(myOptions);

        _userService = new UserService(mongoDbService);
    }

    [Test]
    public async Task AddUser_ShouldAddUserSuccessfully()
    {
        var userRegistrationDto = new UserRegistrationDto
        {
            Name = "John",
            Username = "jMaster123",
            Email = "jMaster123@example.com",
            Password = "password123"
        };

        //var result = await _userService.RegisterUser(userRegistrationDto);

        //Debug.WriteLine("Executed");

        //Assert.IsNotNull(result);
        //Assert.That(result.Email, Is.EqualTo(userRegistrationDto.Email));
    }
}
