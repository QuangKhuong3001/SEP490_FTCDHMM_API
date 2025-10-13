using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Infrastructure.ModelSettings;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Infrastructure.Services
{
    public class MailService : IMailService
    {
        private readonly ILogger _logger;
        private readonly EmailSettings _emailSettings;
        private readonly AppSettings _appSettings;

        public MailService(
            IOptions<EmailSettings> emailOptions,
            IOptions<AppSettings> appOptions,
            ILogger<MailService> logger)
        {
            _logger = logger;
            _emailSettings = emailOptions.Value;
            _appSettings = appOptions.Value;
        }

        public async Task SendEmailAsync(string email, string htmlMessage)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                message.To.Add(MailboxAddress.Parse(email));
                message.Subject = _appSettings.AppName;

                var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Exception found: " + ex.Message);
                throw new AppException(AppResponseCode.SERVICE_NOT_AVAILABLE);
            }
        }
    }
}
