using MelodyMuseAPI.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Errors.Model;
using SendGrid.Helpers.Mail;

namespace MelodyMuseAPI.Services;   

public class EmailSenderService
{
    public AuthMessageSenderSettings Options { get; }
    public EmailSenderService(IOptions<AuthMessageSenderSettings> optionsAccessor)
    {
        Options = optionsAccessor.Value;
    }

    public async Task SendConfirmEmailAsync(string userName, string userEmail, string token)
    {
        if (string.IsNullOrEmpty(Options.SendGridKey))
        {
            throw new Exception("Null SendGridKey");
        }

        var link = $"{Options.BaseURL}/api/auth/c?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(userEmail)}";

        var dynamicTemplateData = new DynamicTemplateData(userName, link);

        await Execute(Options.SendGridKey, Options.TemplateId, dynamicTemplateData, userEmail);
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