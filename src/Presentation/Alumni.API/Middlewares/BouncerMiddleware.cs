using System.Text.Json;
using Alumni.API.Common;

namespace Alumni.API.Middlewares
{
    /// <summary>
    /// "The Bouncer" Middleware (Kapıdaki Güvenlik / Yetki Fedaisi):
    /// Dersin ana başlığı olan "Middleware — the bouncer" mekanizmasını uygular.
    /// Korumalı API rotalarına gelen istekleri kapıda denetler.
    /// Geçerli bir güvenlik başlığı ('X-Api-Key') sunulmamışsa isteği controller'a
    /// sokmadan kapıdan geri çevirir (short-circuit / 401 Unauthorized).
    /// </summary>
    public class BouncerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<BouncerMiddleware> _logger;
        private readonly IConfiguration _configuration;

        // Bouncer denetiminden muaf tutulan genel rotalar (Whitelist)
        private static readonly HashSet<string> PublicPaths = new(StringComparer.OrdinalIgnoreCase)
        {
            "/",
            "/about",
            "/hello",
            "/api/health",
            "/api/test-500",
            "/api/swagger",
            "/openapi/v1.json"
        };

        public BouncerMiddleware(
            RequestDelegate next,
            ILogger<BouncerMiddleware> logger,
            IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? "/";
            var method = context.Request.Method;

            // 1. Genel sayfalar, Swagger dokümantasyonu ve sağlık kontrolü bouncer'a takılmaz
            if (IsPublicPath(path))
            {
                await _next(context);
                return;
            }

            // 2. Bouncer ayarı aktif mi? (appsettings.json veya varsayılan true)
            var bouncerEnabled = _configuration.GetValue<bool>("Bouncer:Enabled", true);
            if (!bouncerEnabled)
            {
                await _next(context);
                return;
            }

            // 3. Razor MVC sayfaları veya salt okunur genel GET istekleri serbestçe geçebilir
            var isApiRoute = path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase);
            var isMutating = HttpMethods.IsPost(method) ||
                             HttpMethods.IsPut(method) ||
                             HttpMethods.IsPatch(method) ||
                             HttpMethods.IsDelete(method);

            // Sadece API üzerinde veri değiştiren işlemler veya korumalı alanlar bouncer kontrolüne tabi tutulur
            if (!isApiRoute || !isMutating)
            {
                await _next(context);
                return;
            }

            // 4. Kapı Kontrolü (Header doğrulaması)
            var headerName = _configuration.GetValue<string>("Bouncer:HeaderName") ?? "X-Api-Key";
            var expectedKey = _configuration.GetValue<string>("Bouncer:ApiKey") ?? "alumni-secret-bouncer-key";

            if (!context.Request.Headers.TryGetValue(headerName, out var providedKey) ||
                string.IsNullOrWhiteSpace(providedKey) ||
                !string.Equals(providedKey, expectedKey, StringComparison.Ordinal))
            {
                _logger.LogWarning("⛔ The Bouncer: Yetkisiz istek kapıdan çevrildi! Rota: {Method} {Path}, IP: {Ip}",
                    method, path, context.Connection.RemoteIpAddress);

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var payload = new
                {
                    status = 401,
                    error = "Unauthorized",
                    bouncer = "Access Denied: The bouncer stopped your request at the door.",
                    reason = $"Geçersiz veya eksik güvenlik başlığı. Bu işlem için '{headerName}' başlığı ile geçerli bir anahtar sağlamalısınız.",
                    path = path,
                    method = method,
                    timestamp = DateTime.UtcNow
                };

                var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                });

                // logs/error.logs dosyasına yetki engellemesini yazma
                await FileErrorLogger.LogErrorAsync(
                    level: "SECURITY",
                    method: method,
                    route: path,
                    statusCode: StatusCodes.Status401Unauthorized,
                    reason: $"The Bouncer engelledi: '{headerName}' güvenlik başlığı eksik veya geçersiz.",
                    details: $"İstemci IP: {context.Connection.RemoteIpAddress}",
                    ip: context.Connection.RemoteIpAddress?.ToString());

                await context.Response.WriteAsync(json);
                return; // Pipeline'ı hemen sonlandır (short-circuit)
            }

            // 5. Anahtar geçerli: Bouncer içeri alıyor
            _logger.LogInformation("✅ The Bouncer: İstek doğrulandı, içeriye aktarılıyor -> {Method} {Path}", method, path);
            await _next(context);
        }

        private static bool IsPublicPath(string path)
        {
            if (PublicPaths.Contains(path)) return true;

            if (path.StartsWith("/api/swagger", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/openapi", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/scalar", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/hello/", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/sum/", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
    }

    public static class BouncerMiddlewareExtensions
    {
        public static IApplicationBuilder UseBouncer(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BouncerMiddleware>();
        }
    }
}

