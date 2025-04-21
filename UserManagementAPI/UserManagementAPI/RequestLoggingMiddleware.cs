using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
namespace UserManagementAPI
{


    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var method = context.Request.Method;
            var path = context.Request.Path;

            switch (method)
            {
                case "POST":
                    _logger.LogInformation($"[POST] Creating a resource: {path}");
                    break;
                case "GET":
                    _logger.LogInformation($"[GET] Data request: {path}");
                    break;
                case "PUT":
                    _logger.LogInformation($"[PUT] Updating the resource: {path}");
                    break;
                case "DELETE":
                    _logger.LogInformation($"[DELETE] Deleting a resource: {path}");
                    break;
                default:
                    _logger.LogInformation($"[{method}] API request: {path}");
                    break;
            }

            await _next(context);
        }
    }

    // Convenient extension for connection
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestLoggingMiddleware>();
        }
    }

}
