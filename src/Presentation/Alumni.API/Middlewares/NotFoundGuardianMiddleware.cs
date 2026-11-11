using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Alumni.API.Common;

namespace Alumni.API.Middlewares
{
    /// <summary>
    /// 404 Guardian Middleware:
    /// "404 guardian middleware - return 404 errors with a reason."
    /// 404 Not Found yanıtlarını yakalar ve hatanın gerekçesini ('reason') 
    /// açık ve yapılandırılmış bir JSON yanıtı olarak döner.
    /// </summary>
    public class NotFoundGuardianMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<NotFoundGuardianMiddleware> _logger;

        public NotFoundGuardianMiddleware(RequestDelegate next, ILogger<NotFoundGuardianMiddleware> logger)
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

                if (context.Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    var path = context.Request.Path.Value ?? "/";
                    var method = context.Request.Method;
                    var endpoint = context.GetEndpoint();

                    // /404 rotasının kendisi çağrıldığında döngüye girmeden sayfayı doğrudan döner
                    if (path.Equals("/404", StringComparison.OrdinalIgnoreCase))
                    {
                        responseBody.Seek(0, SeekOrigin.Begin);
                        await responseBody.CopyToAsync(originalBodyStream);
                        return;
                    }

                    string reason;

                    // Eğer controller veya alt katman gövdeye (body) bir mesaj yazdıysa onu çözümle
                    if (responseBody.Length > 0)
                    {
                        responseBody.Seek(0, SeekOrigin.Begin);
                        using var reader = new StreamReader(responseBody, Encoding.UTF8, leaveOpen: true);
                        var existingBodyText = await reader.ReadToEndAsync();

                        try
                        {
                            var jsonNode = JsonNode.Parse(existingBodyText);
                            if (jsonNode is JsonObject jsonObj)
                            {
                                if (jsonObj.ContainsKey("reason") && jsonObj["reason"] != null)
                                {
                                    // Zaten açık bir 'reason' alanı varsa doğrudan istemciye ilet
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
                                    reason = $"Requested resource at '{path}' was not found.";
                                }
                            }
                            else
                            {
                                reason = string.IsNullOrWhiteSpace(existingBodyText)
                                    ? $"Requested resource at '{path}' was not found."
                                    : existingBodyText;
                            }
                        }
                        catch
                        {
                            reason = string.IsNullOrWhiteSpace(existingBodyText)
                                ? $"Requested resource at '{path}' was not found."
                                : existingBodyText;
                        }
                    }
                    else
                    {
                        // Response body tamamen boş ise (Örn: Rota bulunamadığında veya return NotFound() çağrıldığında)
                        if (endpoint == null)
                        {
                            reason = $"No route matches '{method} {path}'. The requested endpoint does not exist on this server. Please check your URL and HTTP method.";
                        }
                        else
                        {
                            reason = $"The requested resource at endpoint '{path}' was not found.";
                        }
                    }

                    _logger.LogWarning("🛡️ 404 Guardian: {Method} {Path} -> Reason: {Reason}", method, path, reason);

                    // logs/error.logs dosyasına kalıcı 404 uyarısı yazma
                    await FileErrorLogger.LogErrorAsync(
                        level: "WARNING",
                        method: method,
                        route: path,
                        statusCode: StatusCodes.Status404NotFound,
                        reason: reason,
                        details: null,
                        ip: context.Connection.RemoteIpAddress?.ToString());

                    // Görev gereksinimi: "define generic 404 page and redirect invalid request."
                    // Eğer istek tarayıcıdan (HTML) geliyorsa ve /api/* altında değilse: Genel 404 sayfasına yönlendir (Redirect)
                    var isApiRequest = path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase);
                    var acceptsHtml = context.Request.Headers.Accept.Any(a => a != null && a.Contains("text/html"));
                    var isGetOrHead = HttpMethods.IsGet(method) || HttpMethods.IsHead(method);

                    if (!isApiRequest && acceptsHtml && isGetOrHead)
                    {
                        var redirectUrl = $"/404?path={Uri.EscapeDataString(path)}&reason={Uri.EscapeDataString(reason)}";
                        context.Response.Redirect(redirectUrl);
                        return;
                    }

                    // Standart 404 Guardian Yanıtı (Sebep içeren JSON gövdesi)
                    context.Response.ContentType = "application/json";

                    var responsePayload = new
                    {
                        status = 404,
                        error = "Not Found",
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

                    await originalBodyStream.WriteAsync(responseBytes);
                    return;
                }

                // 404 harici tüm durumlar (200, 201, 400 vb.) orijinal akışa aktarılır
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }
    }

    public static class NotFoundGuardianMiddlewareExtensions
    {
        public static IApplicationBuilder UseNotFoundGuardian(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<NotFoundGuardianMiddleware>();
        }
    }
}

