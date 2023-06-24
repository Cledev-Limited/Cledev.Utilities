using System.Net;
using System.Net.Mail;
using Cledev.Server.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cledev.Server.Services;

public interface IEmailService
{
    Task SendEmail(string address, string subject, string htmlMessage);
}

public class EmailService : IEmailService
{
    private readonly MailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    [ActivatorUtilitiesConstructor]
    public EmailService(IOptions<MailSettings> settings, ILogger<EmailService> logger) : this(settings.Value, logger)
    {
    }
    
    public EmailService(MailSettings settings, ILogger<EmailService> logger)
    {
        _settings = settings;
        _logger = logger;
    }

    public async Task SendEmail(string address, string subject, string htmlMessage)
    {
        try
        {
            var message = new MailMessage
            {
                From = new MailAddress(_settings.Address, _settings.DisplayName),
                Subject = subject,
                IsBodyHtml = true,
                Body = htmlMessage
            };

            message.To.Add(new MailAddress(address));

            var smtp = new SmtpClient
            {
                Port = _settings.Port,
                Host = _settings.Host,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_settings.Address, _settings.Password),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            await smtp.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
    }
}
