using Lingua.ZoomIntegration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Lingua.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigController : ControllerBase
    {
        private readonly ZoomClientOptions _zoomClientOptions;

        public ConfigController(IOptions<ZoomClientOptions> zoomClientOptions)
        {
            _zoomClientOptions = zoomClientOptions.Value;
        }

        [HttpGet]
        [Route("zoom")]
        public IActionResult ZoomOAuthUrl()
        {
            return Ok(_zoomClientOptions.OAuthUrl);
        }

        [HttpGet]
        [Route("languages")]
        public IActionResult Languages()
        {
            return Ok(new List<string> { "English", "Russian", "German", "French", "Spanish", "Japanese", "Italian", "Chinese", "Korean" });
        }

        [HttpGet]
        [Route("languagelevels")]
        public IActionResult LanguageLevels()
        {
            return Ok(new[]
            {
                new {  Code = "A1", Description = "Beginner"},
                new {  Code = "A2", Description = "Elementary"},
                new {  Code = "B1", Description = "Intermediate"},
                new {  Code = "B2", Description = "Upper Intermediate"},
                new {  Code = "C1", Description = "Advanced"},
                new {  Code = "C2", Description = "Proficient"},
            });
        }
    }
}
