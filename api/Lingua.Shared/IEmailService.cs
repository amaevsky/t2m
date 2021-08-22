using System.Threading.Tasks;

namespace Lingua.Shared
{
    public interface IEmailService
    {
        Task SendAsync(EmailMessage emailMessage, params string[] recipients);
    }
}
