using Lingua.Shared;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Lingua.Services
{
    public class GmailService : IEmailService
    {
        public async Task SendAsync(string subject, string body, params string[] recipients)
        {
            var fromAddress = new MailAddress("maevsky.artem@gmail.com", "Lingua app");
            const string fromPassword = "5764xxxx";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };


            foreach (var recepient in recipients)
            {
                var toAddress = new MailAddress(recepient);

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,

                })
                {
                    await smtp.SendMailAsync(message);
                }
            }
        }
    }
}
