using NUnit.Framework;
using Moq;
using MelodyMuseAPI.Models;
using MongoDB.Driver;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MelodyMuseAPI_DotNet8.Data;
using MelodyMuseAPI_DotNet8.Dtos;
using MelodyMuseAPI_DotNet8.Services;
using MelodyMuseAPI_DotNet8.Interfaces;
using System.Xml.Linq;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

[TestFixture]
public class UserUnitTest
{
    private readonly IUserService _userService;

    public UserUnitTest()
    {
        var services = new ServiceCollection();
        services.AddTransient<IUserService, UserService>();

        var serviceProvider = services.BuildServiceProvider();

        _userService = serviceProvider.GetService<IUserService>();
    }

    [SetUp]
    public void Setup()
    {
       
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

        var result = await _userService.RegisterUser(userRegistrationDto);

        Debug.WriteLine("Executed");

        Assert.IsNotNull(result);
        Assert.That(result.Email, Is.EqualTo(userRegistrationDto.Email));
    }
}
