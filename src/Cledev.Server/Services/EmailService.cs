using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cledev.Server.Services;

public class EMailSettings
{
    public string Address { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Host { get; set; } = null!;
    public int Port { get; set; }
}

public interface IEmailService
{
    Task SendEmail(string subject, string body, string toAddress);
    
    Task SendEmail(
        string subject, 
        string body, 
        IEnumerable<string> toAddresses, 
        IEnumerable<string>? ccAddresses = null, 
        IEnumerable<string>? bccAddresses = null,
        MailPriority? priority = null,
        IEnumerable<Attachment>? attachments = null,
        IDictionary<string, string>? emailHeaders = null);
}

public class EmailService : IEmailService
{
    private readonly EMailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    [ActivatorUtilitiesConstructor]
    public EmailService(IOptions<EMailSettings> settings, ILogger<EmailService> logger) : this(settings.Value, logger)
    {
    }
    
    public EmailService(EMailSettings settings, ILogger<EmailService> logger)
    {
        _settings = settings;
        _logger = logger;
    }

    public async Task SendEmail(string subject, string body, string toAddress)
    {
        await SendEmail(subject, body, new List<string> { toAddress });
    }
    
    public async Task SendEmail(
        string subject, 
        string body, 
        IEnumerable<string> toAddresses, 
        IEnumerable<string>? ccAddresses = null, 
        IEnumerable<string>? bccAddresses = null,
        MailPriority? priority = null,
        IEnumerable<Attachment>? attachments = null,
        IDictionary<string, string>? emailHeaders = null)
    {
        try
        {
            var message = new MailMessage
            {
                From = new MailAddress(_settings.Address, _settings.DisplayName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
                Priority = priority ?? MailPriority.Normal
            };

            toAddresses.ToList().ForEach(address => message.To.Add(new MailAddress(address)));
            ccAddresses?.ToList().ForEach(address => message.CC.Add(new MailAddress(address)));
            bccAddresses?.ToList().ForEach(address => message.Bcc.Add(new MailAddress(address)));
            emailHeaders?.ToList().ForEach(header => message.Headers.Add(header.Key, header.Value));
            attachments?.ToList().ForEach(attachment => message.Attachments.Add(attachment));

            var client = new SmtpClient
            {
                Port = _settings.Port,
                Host = _settings.Host,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_settings.Address, _settings.Password),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            await client.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
    }
}
