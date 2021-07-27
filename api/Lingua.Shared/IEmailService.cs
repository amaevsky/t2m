using System.Threading.Tasks;

namespace Lingua.Shared
{
    public interface IEmailService
    {
        Task SendAsync(string subject, string body, params string[] recipients);
    }
}
