using System.Diagnostics;
using Alumni.API.Common;

namespace Alumni.API.Middlewares
{
    /// <summary>
    /// Gelen her HTTP isteğini terminale formatlanmış olarak loglayan Middleware.
    /// Format: time , method , route , response type , duration
    /// Örnek: 10:20:30 , POST , /users , 201 , 18ms
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                var time = DateTime.Now.ToString("HH:mm:ss");
                var method = context.Request.Method;
                var route = context.Request.Path.Value ?? "/";
                var statusCode = context.Response.StatusCode;
                var duration = $"{stopwatch.ElapsedMilliseconds}ms";

                // Format: time , method , route , response type , duration
                var logLine = $"{time} , {method} , {route} , {statusCode} , {duration}";

                // Terminale doğrudan ham çıktı yazdırma (Görev gereksinimi)
                Console.WriteLine(logLine);

                // Standart ILogger altyapısına da bildirme
                _logger.LogInformation("{LogLine}", logLine);

                // 404, 401 ve 5xx dışındaki diğer istemci hatalarını da (400, 409 vb.) logs/error.logs'a kaydet
                if (statusCode >= 400 && statusCode != 404 && statusCode != 401 && statusCode < 500)
                {
                    _ = FileErrorLogger.LogErrorAsync(
                        "CLIENT_ERROR",
                        method,
                        route,
                        statusCode,
                        $"HTTP {statusCode} durum kodu istemciye dönüldü.",
                        null,
                        context.Connection.RemoteIpAddress?.ToString());
                }
            }
        }
    }

    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}

