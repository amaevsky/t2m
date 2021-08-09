using Lingua.Services;
using Lingua.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Lingua.API
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "text/plain";
            string response;
            if (exception is ValidationException validationEx)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = validationEx.UserFriendlyMessage;
            }
            else
            {
                _logger.LogError(exception.ToString());
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = "Internal server error occured.";
            }

            await context.Response.WriteAsync(response);
        }
    }
}


