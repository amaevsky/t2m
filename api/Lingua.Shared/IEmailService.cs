using System.Threading.Tasks;

namespace Lingua.Shared
{
    public interface IEmailService
    {
        Task SendAsync(string subject, string body, bool isHtml, params string[] recipients);
    }
}
