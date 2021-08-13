using System.Threading.Tasks;

namespace Lingua.EmailTemplates
{
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync(string viewName, object model);
    }
}
