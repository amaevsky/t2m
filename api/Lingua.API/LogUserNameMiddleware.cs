using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Lingua.API
{
    public class LogUserNameMiddleware
    {
        private readonly RequestDelegate next;

        public LogUserNameMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public Task Invoke(HttpContext context)
        {
            var email = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (email != null)
            {
                LogContext.PushProperty("User", email);
            }

            return next(context);
        }
    }
}


