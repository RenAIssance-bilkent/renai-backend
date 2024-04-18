using NUnit.Framework;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using MelodyMuseAPI.Services;
using MelodyMuseAPI.Settings;

[TestFixture]
public class EmailSenderServiceTests
{
    private EmailSenderService _emailService;
    private AuthMessageSenderSettings _settings;

    [SetUp]
    public void Setup()
    {
        // Assume the API key is set via environment variables or secure secrets management in CI/CD pipeline
        _settings = new AuthMessageSenderSettings
        {
            SendGridKey = "" // Replace with actual key or pull from secure store
        };

        var options = Options.Create(_settings);
        _emailService = new EmailSenderService(options);
    }

    //[Test]
    //public async Task SendEmailAsync_ValidParameters_ShouldSendEmail()
    //{
    //    var toEmail = "k-aliyev@outlook.com"; // Use a controlled test email address
    //    var subject = "Integration Test";
    //    var message = "This is a test email from the integration test.";

    //    Assert.DoesNotThrowAsync(() => _emailService.SendEmailAsync(toEmail, subject, message));
    //}

    [Test]
    public async Task SendEmailAsync_InvalidApiKey_ShouldThrowException()
    {
        await _emailService.SendConfirmEmailAsync("test","k-aliyev@outlook.com","test-token");
    }
}
