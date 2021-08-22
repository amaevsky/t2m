using RazorLight;
using System.Threading.Tasks;

namespace Lingua.EmailTemplates
{
    public class ViewRenderService : IViewRenderService
    {
        private readonly RazorLightEngine _engine;
        public ViewRenderService()
        {
            _engine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(GetType().Assembly)
                .UseMemoryCachingProvider()
                .Build();
        }

        public async Task<string> RenderToStringAsync(string viewName, object model)
        {
            var key = $"Views.{viewName}";
            string result = await _engine.CompileRenderAsync(key, model);
            return result;
        }
    }
}
