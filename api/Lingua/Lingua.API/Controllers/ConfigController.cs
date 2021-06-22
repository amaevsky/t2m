using Lingua.ZoomIntegration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Lingua.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfigControler : ControllerBase
    {
        private readonly ZoomClientOptions _zoomClientOptions;

        public ConfigControler(IOptions<ZoomClientOptions> zoomClientOptions)
        {
            _zoomClientOptions = zoomClientOptions.Value;
        }

        [HttpGet]
        [Route("zoom")]
        public IActionResult ZoomOAuthUrl()
        {
            return Ok(_zoomClientOptions.OAuthUrl);
        }
    }
}
