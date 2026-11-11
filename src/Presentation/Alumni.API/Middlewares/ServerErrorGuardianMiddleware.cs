using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Alumni.API.Common;

namespace Alumni.API.Middlewares
{
    /// <summary>
    /// 5xx Guardian Middleware:
    /// "5xx guardian middleware - return 500/5xx errors with a reason."
    /// Sunucu kaynaklı 5xx (500 Internal Server Error, 502, 503 vb.) hataları ve
    /// boru hattında fırlatılan tüm beklenmeyen istisnaları (unhandled exceptions) yakalar;
    /// istemciye hatanın gerekçesini ('reason') açık ve yapılandırılmış bir JSON yanıtı olarak döner.
    /// </summary>
    public class ServerErrorGuardianMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ServerErrorGuardianMiddleware> _logger;

        public ServerErrorGuardianMiddleware(
            RequestDelegate next,
            ILogger<ServerErrorGuardianMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);

                // Eğer durum kodu 5xx aralığındaysa (500 - 599)
                if (context.Response.StatusCode >= StatusCodes.Status500InternalServerError &&
                    context.Response.StatusCode <= 599)
                {
                    await HandleServerErrorResponseAsync(context, originalBodyStream, responseBody, null);
                    return;
                }

                // Normal yanıtı orijinal akışa kopyala
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
            catch (Exception ex)
            {
                // Boru hattında yakalanmayan tüm istisnalar (Unhandled Exception -> 500)
                _logger.LogError(ex, "🛡️ 5xx Guardian: Beklenmeyen bir sunucu hatası yakalandı! Rota: {Method} {Path}",
                    context.Request.Method, context.Request.Path);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await HandleServerErrorResponseAsync(context, originalBodyStream, responseBody, ex);
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }

        private async Task HandleServerErrorResponseAsync(
            HttpContext context,
            Stream originalBodyStream,
            MemoryStream responseBody,
            Exception? exception)
        {
            var path = context.Request.Path.Value ?? "/";
            var method = context.Request.Method;
            var statusCode = context.Response.StatusCode >= 500 ? context.Response.StatusCode : 500;

            string reason;

            if (exception != null)
            {
                reason = !string.IsNullOrWhiteSpace(exception.Message)
                    ? exception.Message
                    : "An unexpected internal server error occurred.";
            }
            else if (responseBody.Length > 0)
            {
                responseBody.Seek(0, SeekOrigin.Begin);
                using var reader = new StreamReader(responseBody, Encoding.UTF8, leaveOpen: true);
                var existingText = await reader.ReadToEndAsync();

                try
                {
                    var jsonNode = JsonNode.Parse(existingText);
                    if (jsonNode is JsonObject jsonObj)
                    {
                        if (jsonObj.ContainsKey("reason") && jsonObj["reason"] != null)
                        {
                            responseBody.Seek(0, SeekOrigin.Begin);
                            await responseBody.CopyToAsync(originalBodyStream);
                            return;
                        }

                        if (jsonObj.ContainsKey("message") && jsonObj["message"] != null)
                        {
                            reason = jsonObj["message"]!.ToString();
                        }
                        else
                        {
                            reason = "An internal server error occurred.";
                        }
                    }
                    else
                    {
                        reason = string.IsNullOrWhiteSpace(existingText) ? "Internal server error." : existingText;
                    }
                }
                catch
                {
                    reason = string.IsNullOrWhiteSpace(existingText) ? "Internal server error." : existingText;
                }
            }
            else
            {
                reason = "An unexpected internal server error occurred on the server.";
            }

            context.Response.ContentType = "application/json";

            var responsePayload = new
            {
                status = statusCode,
                error = GetErrorTitle(statusCode),
                reason = reason,
                path = path,
                method = method,
                timestamp = DateTime.UtcNow
            };

            var jsonString = JsonSerializer.Serialize(responsePayload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            var responseBytes = Encoding.UTF8.GetBytes(jsonString);
            context.Response.ContentLength = responseBytes.Length;

            _logger.LogWarning("🛡️ 5xx Guardian: {StatusCode} {Method} {Path} -> Reason: {Reason}",
                statusCode, method, path, reason);

            // logs/error.logs dosyasına kalıcı hata kaydı yazma
            await FileErrorLogger.LogErrorAsync(
                level: "ERROR",
                method: method,
                route: path,
                statusCode: statusCode,
                reason: reason,
                details: exception != null ? $"{exception.GetType().FullName}: {exception.Message}\n{exception.StackTrace}" : null,
                ip: context.Connection.RemoteIpAddress?.ToString());

            await originalBodyStream.WriteAsync(responseBytes);
        }

        private static string GetErrorTitle(int statusCode) => statusCode switch
        {
            500 => "Internal Server Error",
            501 => "Not Implemented",
            502 => "Bad Gateway",
            503 => "Service Unavailable",
            504 => "Gateway Timeout",
            _ => "Server Error"
        };
    }

    public static class ServerErrorGuardianMiddlewareExtensions
    {
        public static IApplicationBuilder UseServerErrorGuardian(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ServerErrorGuardianMiddleware>();
        }

        public static IApplicationBuilder Use5xxGuardian(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ServerErrorGuardianMiddleware>();
        }
    }
}

