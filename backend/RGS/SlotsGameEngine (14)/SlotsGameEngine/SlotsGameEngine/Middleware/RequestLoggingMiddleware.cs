using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using SlotsGameEngine.BL.BLL.Interfaces;

namespace SlotsGameEngine.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context, ILoggingService logging)
        {
            var endpoint = context.Request.Path.ToString();
            var method = context.Request.Method;

            await _next(context);

            // minimal log (body omitted to keep it simple)
            await logging.LogApiCallAsync(
                endpoint,
                method,
                requestBody: null,
                responseBody: null,
                statusCode: context.Response?.StatusCode,
                error: null,
                context.RequestAborted);
        }
    }

    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
            => app.UseMiddleware<RequestLoggingMiddleware>();
    }
}
