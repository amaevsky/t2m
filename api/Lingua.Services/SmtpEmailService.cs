using Lingua.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Lingua.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly SmtpSettings _settings;
        private readonly SmtpClient _client;
        private readonly ILogger _logger;

        public SmtpEmailService(IOptions<SmtpSettings> settings, ILogger<SmtpEmailService> logger)
        {
            _settings = settings.Value;
            _client = new SmtpClient
            {
                Host = _settings.Host,
                Port = _settings.Port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_settings.Email, _settings.Password)
            };

            _client.SendCompleted += OnSendCompleted;
            _logger = logger;
        }

        private void OnSendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                _logger.LogError(e.Error, "Failed to send a message");
            }
        }

        public async Task SendAsync(string subject, string body, params string[] recipients)
        {
            var fromAddress = new MailAddress(_settings.Email);

            foreach (var recepient in recipients)
            {
                var toAddress = new MailAddress(recepient);

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,

                })
                {
                    await _client.SendMailAsync(message);
                }
            }
        }
    }
}
