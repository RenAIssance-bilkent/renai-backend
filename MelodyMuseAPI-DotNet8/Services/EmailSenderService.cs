using MelodyMuseAPI.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Errors.Model;
using SendGrid.Helpers.Mail;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MelodyMuseAPI.Services;   

public class EmailSenderService
{
    public AuthMessageSenderSettings Options { get; }
    private readonly IOptions<JwtSettings> _jwtSettings;
    public EmailSenderService(IOptions<AuthMessageSenderSettings> optionsAccessor, IOptions<JwtSettings> jwtSettings)
    {
        Options = optionsAccessor.Value;
        _jwtSettings = jwtSettings;
    }

    public string GenerateConfirmationToken()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddDays(1);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                    new Claim("action", "email_confirmation")
            }),
            Expires = expires,
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task SendWelcomeConfirmationEmail(string userName, string userEmail, string token)
    {
        await SendEmailAsync(userName, userEmail, token, 0);
    }

    public async Task SendEditConfirmationEmail(string userName, string userEmail, string token)
    {
        await SendEmailAsync(userName, userEmail, token, 1);
    }

    public async Task SendResetPasswordEmail(string userName, string userEmail, string newPassword)
    {
        await SendEmailAsync(userName, userEmail, newPassword, 2);
    }

    private async Task SendEmailAsync(string userName, string userEmail, string tokenOrPassword, int type)
    {
        if (string.IsNullOrEmpty(Options.SendGridKey))
        {
            throw new Exception("Null SendGridKey");
        }

        string templateId;
        string content;

        switch (type)
        {
            case 0: // Welcome Email
                templateId = Options.WelcomeTemplateId;
                content = $"{Options.BaseURL}/api/auth/c?token={Uri.EscapeDataString(tokenOrPassword)}&email={Uri.EscapeDataString(userEmail)}";
                break;
            case 1: // Confirmation Email
                templateId = Options.ConfirmTemplateId;
                content = $"{Options.BaseURL}/api/auth/c?token={Uri.EscapeDataString(tokenOrPassword)}&email={Uri.EscapeDataString(userEmail)}";
                break;
            case 2: // Reset Password Email
                templateId = Options.ResetTemplateId;
                content = tokenOrPassword; // Here content is the new password.
                break;
            default:
                throw new ArgumentException("Invalid email type.");
        }

        var dynamicTemplateData = new DynamicTemplateData(userName, content);
        await Execute(Options.SendGridKey, templateId, dynamicTemplateData, userEmail);
    }

    public async Task Execute(string apiKey, string templateId, DynamicTemplateData dynamicTemplateData, string toEmail)
    {
        var client = new SendGridClient(apiKey);
        var from = new EmailAddress("k.aliyev@ug.bilkent.edu.tr", "MelodyMuse@noreply"); // TODO: Remove hardcoded
        var to = new EmailAddress(toEmail);

        var msg = new SendGridMessage();

        msg.SetFrom(from);
        msg.AddTo(to);
        msg.SetTemplateId(templateId);

        msg.SetTemplateData(dynamicTemplateData);

        var response = await client.SendEmailAsync(msg);

    }
}

public class DynamicTemplateData
{
    [JsonProperty("name")]
    public string RecipientName { get; set; }
    [JsonProperty("link")]
    public string ConfirmationLink { get; set; }

    public DynamicTemplateData(string recipientName, string confirmationLink)
    {
        RecipientName = recipientName;
        ConfirmationLink = confirmationLink;
    }
}