using System.Text.Json;
using System.Text.RegularExpressions;
using Alumni.API.Common;

namespace Alumni.API.Middlewares
{
    /// <summary>
    /// 9. Hafta Auth Middleware ("The Bouncer Checks The Card" & "Who May Do What"):
    /// - Validasyondan (400) ÖNCE çalışır: kart yoksa / bozuksa / süresi dolmuşsa 401 { "error": "login required" } döner.
    /// - Rol ve sahiplik (RBAC + own/self) kurallarını DATA_DESIGN.md §4 yetki matrisine göre tek merkezden uygular:
    ///   - 401 Unauthorized: "I do not know who you are"
    ///   - 403 Forbidden:    "I know who you are, and the answer is no"
    /// - Kural yalnızca arayüz butonunu gizlemek değildir ("the rule is written once · the button is not the rule").
    /// </summary>
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthMiddleware> _logger;

        // API route kalıpları
        private static readonly Regex ApiUsersByIdRegex = new(@"^/api/users/(?<id>\d+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex ApiUserJobsRegex = new(@"^/api/users/(?<id>\d+)/jobs$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex ApiUserApplicationsRegex = new(@"^/api/users/(?<id>\d+)/applications$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex ApiJobsByIdRegex = new(@"^/api/jobs/(?<id>\d+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex ApiJobApplicationsRegex = new(@"^/api/jobs/(?<id>\d+)/applications$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex ApiApplicationsByIdRegex = new(@"^/api/applications/(?<id>\d+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex ApiApplicationStatusRegex = new(@"^/api/applications/(?<id>\d+)/status$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex ApiAnnouncementsByIdRegex = new(@"^/api/announcements/(?<id>\d+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // MVC route kalıpları
        private static readonly Regex MvcUsersEditRegex = new(@"^/users?/edit/(?<id>\d+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex MvcUsersDeleteRegex = new(@"^/users?/delete/(?<id>\d+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex MvcUsersDetailsRegex = new(@"^/users?/details/(?<id>\d+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex MvcJobsDetailsRegex = new(@"^/(?:jobs|job-posts)/details/(?<id>\d+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex MvcJobsApplyRegex = new(@"^/(?:jobs|job-posts)/(?<id>\d+)/apply$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex MvcJobsDeleteRegex = new(@"^/(?:jobs|job-posts)/delete/(?<id>\d+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex MvcAnnouncementsDetailsRegex = new(@"^/announcements?/details/(?<id>\d+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex MvcAnnouncementsEditRegex = new(@"^/announcements?/edit/(?<id>\d+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex MvcAnnouncementsDeleteRegex = new(@"^/announcements?/delete/(?<id>\d+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex MvcApplicationsStatusRegex = new(@"^/applications/(?<id>\d+)/status$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex MvcApplicationsDeleteRegex = new(@"^/applications/delete/(?<id>\d+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public AuthMiddleware(RequestDelegate next, ILogger<AuthMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var rawPath = context.Request.Path.Value ?? "/";
            var path = rawPath.Length > 1 ? rawPath.TrimEnd('/') : rawPath;
            var method = context.Request.Method.ToUpperInvariant();
            var isApi = path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase);

            var isAuthenticated = context.User.IsAuthenticatedUser();
            var currentUserId = context.User.GetUserId();
            var currentRole = context.User.GetUserRole();
            var isAdmin = context.User.IsAdminRole();

            if (isAuthenticated)
            {
                context.Items["AuthUserId"] = currentUserId!.Value;
                context.Items["AuthUserRole"] = currentRole;
                context.Items["AuthUserName"] = context.User.GetUserName();
            }

            // 1. Genel / altyapı / auth uç noktaları (herkese açık)
            if (IsAlwaysPublic(path, method))
            {
                await _next(context);
                return;
            }

            // 2. SIGN UP (guest: yes, graduate: no, admin: yes)
            if (IsSignUpRoute(path, method))
            {
                if (isAuthenticated && !isAdmin)
                {
                    await DenyForbiddenAsync(context, isApi, "Oturum açmış mezun kullanıcılar tekrar kayıt (sign up) işlemi yapamaz.");
                    return;
                }

                await _next(context);
                return;
            }

            // 3. SEE JOB POSTINGS (guest: yes, graduate: yes, admin: yes)
            if (IsPublicJobReadRoute(path, method))
            {
                await _next(context);
                return;
            }

            // 4. SEE ANNOUNCEMENTS (guest: yes, graduate: yes, admin: yes)
            if (IsPublicAnnouncementReadRoute(path, method))
            {
                await _next(context);
                return;
            }

            // 5. ADMIN PANEL (guest: no -> 401/login, graduate: no -> 403, admin: yes)
            if (path.Equals("/admin", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/admin/", StringComparison.OrdinalIgnoreCase) ||
                path.Equals("/api/admin", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/api/admin/", StringComparison.OrdinalIgnoreCase))
            {
                if (!isAuthenticated)
                {
                    await DenyUnauthorizedAsync(context, isApi);
                    return;
                }

                if (!isAdmin)
                {
                    await DenyForbiddenAsync(context, isApi, "Admin Paneli yalnızca Admin rolündeki kullanıcılara açıktır. Mevcut rolünüzle bu alana erişemezsiniz.");
                    return;
                }

                await _next(context);
                return;
            }

            // 6. USER-SPECIFIC PERSONAL PAGES (/profile, /settings — yalnızca oturum açmış kullanıcı kendi hesabını görür)
            if (path.Equals("/profile", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/profile/", StringComparison.OrdinalIgnoreCase) ||
                path.Equals("/settings", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/settings/", StringComparison.OrdinalIgnoreCase))
            {
                if (!isAuthenticated)
                {
                    await DenyUnauthorizedAsync(context, isApi);
                    return;
                }

                await _next(context);
                return;
            }

            // 7. GET /api/me (guest: no -> 401, graduate/admin: yes)
            if (path.Equals("/api/me", StringComparison.OrdinalIgnoreCase) && method == "GET")
            {
                if (!isAuthenticated)
                {
                    await DenyUnauthorizedAsync(context, isApi);
                    return;
                }

                await _next(context);
                return;
            }

            // 6. POST A JOB (guest: no -> 401, graduate: own -> başkası için 403, admin: yes)
            var apiUserJobsMatch = ApiUserJobsRegex.Match(path);
            if (apiUserJobsMatch.Success && method == "POST")
            {
                if (!isAuthenticated)
                {
                    await DenyUnauthorizedAsync(context, isApi);
                    return;
                }

                var targetUserId = int.Parse(apiUserJobsMatch.Groups["id"].Value);
                if (!isAdmin && targetUserId != currentUserId!.Value)
                {
                    await DenyForbiddenAsync(context, isApi, $"Kullanıcı #{currentUserId} başka bir kullanıcı (#{targetUserId}) adına ilan açamaz.");
                    return;
                }

                await _next(context);
                return;
            }

            if ((path.Equals("/jobs/create", StringComparison.OrdinalIgnoreCase) ||
                 path.Equals("/job-posts/create", StringComparison.OrdinalIgnoreCase)) && method == "POST")
            {
                if (!isAuthenticated)
                {
                    await DenyUnauthorizedAsync(context, isApi);
                    return;
                }

                await _next(context);
                return;
            }

            // 7. APPLY TO A JOB (guest: no -> 401, graduate: self, admin: no -> 403)
            var apiJobAppMatch = ApiJobApplicationsRegex.Match(path);
            if ((apiJobAppMatch.Success && method == "POST") ||
                (path.Equals("/api/applications", StringComparison.OrdinalIgnoreCase) && method == "POST") ||
                (MvcJobsApplyRegex.IsMatch(path) && method == "POST") ||
                (path.Equals("/applications/create", StringComparison.OrdinalIgnoreCase) && method == "POST"))
            {
                if (!isAuthenticated)
                {
                    await DenyUnauthorizedAsync(context, isApi);
                    return;
                }

                if (isAdmin)
                {
                    await DenyForbiddenAsync(context, isApi, "Admin rolündeki kullanıcılar iş ilanlarına başvuru yapamaz.");
                    return;
                }

                await _next(context);
                return;
            }

            // 8. DELETE A USER (guest: no -> 401, graduate: no -> 403, admin: yes)
            var apiUserByIdMatch = ApiUsersByIdRegex.Match(path);
            if ((apiUserByIdMatch.Success && method == "DELETE") ||
                (MvcUsersDeleteRegex.IsMatch(path) && method == "POST"))
            {
                if (!isAuthenticated)
                {
                    await DenyUnauthorizedAsync(context, isApi);
                    return;
                }

                if (!isAdmin)
                {
                    await DenyForbiddenAsync(context, isApi, "Kullanıcı silme yetkisi yalnızca Admin rolüne aittir.");
                    return;
                }

                await _next(context);
                return;
            }

            // 9. CHANGE A PROFILE (guest: no -> 401, graduate: own -> başkası için 403, admin: yes)
            if (apiUserByIdMatch.Success && (method == "PUT" || method == "PATCH"))
            {
                if (!isAuthenticated)
                {
                    await DenyUnauthorizedAsync(context, isApi);
                    return;
                }

                var targetUserId = int.Parse(apiUserByIdMatch.Groups["id"].Value);
                if (!isAdmin && targetUserId != currentUserId!.Value)
                {
                    await DenyForbiddenAsync(context, isApi, $"Kullanıcı #{currentUserId} başka bir kullanıcının (#{targetUserId}) profilini değiştiremez.");
                    return;
                }

                await _next(context);
                return;
            }

            var mvcUserEditMatch = MvcUsersEditRegex.Match(path);
            if (mvcUserEditMatch.Success && (method == "GET" || method == "POST"))
            {
                if (!isAuthenticated)
                {
                    await DenyUnauthorizedAsync(context, isApi);
                    return;
                }

                var targetUserId = int.Parse(mvcUserEditMatch.Groups["id"].Value);
                if (!isAdmin && targetUserId != currentUserId!.Value)
                {
                    await DenyForbiddenAsync(context, isApi, $"Kullanıcı #{currentUserId} başka bir kullanıcının (#{targetUserId}) profilini değiştiremez.");
                    return;
                }

                await _next(context);
                return;
            }

            // 10. SEE USERS (guest: no -> 401, graduate: yes, admin: yes)
            if ((path.Equals("/api/users", StringComparison.OrdinalIgnoreCase) && method == "GET") ||
                (apiUserByIdMatch.Success && method == "GET") ||
                (apiUserJobsMatch.Success && method == "GET") ||
                ((path.Equals("/users", StringComparison.OrdinalIgnoreCase) || path.Equals("/user", StringComparison.OrdinalIgnoreCase)) && method == "GET") ||
                (MvcUsersDetailsRegex.IsMatch(path) && method == "GET"))
            {
                if (!isAuthenticated)
                {
                    await DenyUnauthorizedAsync(context, isApi);
                    return;
                }

                await _next(context);
                return;
            }

            // 11. USER APPLICATIONS (guest: no -> 401, graduate: own -> 403, admin: yes)
            var apiUserAppsMatch = ApiUserApplicationsRegex.Match(path);
            if (apiUserAppsMatch.Success && method == "GET")
            {
                if (!isAuthenticated)
                {
                    await DenyUnauthorizedAsync(context, isApi);
                    return;
                }

                var targetUserId = int.Parse(apiUserAppsMatch.Groups["id"].Value);
                if (!isAdmin && targetUserId != currentUserId!.Value)
                {
                    await DenyForbiddenAsync(context, isApi, "Başka bir kullanıcının başvurularını görüntüleyemezsiniz.");
                    return;
                }

                await _next(context);
                return;
            }

            // 12. JOB APPLICANTS & DELETE JOB (guest: no -> 401; sahiplik controller'da job.PostedByUserId ile kontrol edilir)
            if ((apiJobAppMatch.Success && method == "GET") ||
                (ApiJobsByIdRegex.IsMatch(path) && method == "DELETE") ||
                (MvcJobsDeleteRegex.IsMatch(path) && method == "POST"))
            {
                if (!isAuthenticated)
                {
                    await DenyUnauthorizedAsync(context, isApi);
                    return;
                }

                await _next(context);
                return;
            }

            // 13. MANAGE ANNOUNCEMENTS (guest: no -> 401, graduate: no -> 403, admin: yes)
            if ((path.Equals("/api/announcements", StringComparison.OrdinalIgnoreCase) && method == "POST") ||
                (ApiAnnouncementsByIdRegex.IsMatch(path) && (method == "PUT" || method == "PATCH" || method == "DELETE")) ||
                ((path.Equals("/announcements", StringComparison.OrdinalIgnoreCase) ||
                  path.Equals("/announcement", StringComparison.OrdinalIgnoreCase) ||
                  path.Equals("/announcements/create", StringComparison.OrdinalIgnoreCase) ||
                  path.Equals("/announcement/create", StringComparison.OrdinalIgnoreCase)) && method == "POST") ||
                (MvcAnnouncementsEditRegex.IsMatch(path) && (method == "GET" || method == "POST")) ||
                (MvcAnnouncementsDeleteRegex.IsMatch(path) && method == "POST"))
            {
                if (!isAuthenticated)
                {
                    await DenyUnauthorizedAsync(context, isApi);
                    return;
                }

                if (!isAdmin)
                {
                    await DenyForbiddenAsync(context, isApi, "Duyuru yönetim yetkisi yalnızca Admin rolüne aittir.");
                    return;
                }

                await _next(context);
                return;
            }

            // 14. APPLICATIONS LIST / DETAIL / STATUS / DELETE
            if (path.Equals("/api/applications", StringComparison.OrdinalIgnoreCase) ||
                ApiApplicationsByIdRegex.IsMatch(path) ||
                ApiApplicationStatusRegex.IsMatch(path) ||
                path.Equals("/applications", StringComparison.OrdinalIgnoreCase) ||
                MvcApplicationsStatusRegex.IsMatch(path) ||
                MvcApplicationsDeleteRegex.IsMatch(path))
            {
                if (!isAuthenticated)
                {
                    await DenyUnauthorizedAsync(context, isApi);
                    return;
                }

                if ((ApiApplicationStatusRegex.IsMatch(path) && method == "PATCH") ||
                    (MvcApplicationsStatusRegex.IsMatch(path) && method == "POST"))
                {
                    if (!isAdmin)
                    {
                        await DenyForbiddenAsync(context, isApi, "Başvuru durumunu yalnızca Admin güncelleyebilir.");
                        return;
                    }
                }

                await _next(context);
                return;
            }

            // 15. Güvenlik ağı: Tanımlanmamış diğer tüm /api/* yazma istekleri oturum gerektirir
            if (isApi && !isAuthenticated && IsMutatingMethod(method))
            {
                await DenyUnauthorizedAsync(context, isApi);
                return;
            }

            await _next(context);
        }

        private static bool IsAlwaysPublic(string path, string method)
        {
            if (path == "/" ||
                path.Equals("/about", StringComparison.OrdinalIgnoreCase) ||
                path.Equals("/hello", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/hello/", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/sum/", StringComparison.OrdinalIgnoreCase) ||
                path.Equals("/404", StringComparison.OrdinalIgnoreCase) ||
                path.Equals("/api/health", StringComparison.OrdinalIgnoreCase) ||
                path.Equals("/api/test-500", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/api/swagger", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/openapi", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/scalar", StringComparison.OrdinalIgnoreCase) ||
                path.Equals("/login", StringComparison.OrdinalIgnoreCase) ||
                path.Equals("/auth/login", StringComparison.OrdinalIgnoreCase) ||
                path.Equals("/logout", StringComparison.OrdinalIgnoreCase) ||
                path.Equals("/auth/logout", StringComparison.OrdinalIgnoreCase) ||
                (path.Equals("/api/login", StringComparison.OrdinalIgnoreCase) && method == "POST") ||
                (path.Equals("/api/logout", StringComparison.OrdinalIgnoreCase) && method == "POST"))
            {
                return true;
            }

            return false;
        }

        private static bool IsSignUpRoute(string path, string method)
        {
            return (path.Equals("/api/register", StringComparison.OrdinalIgnoreCase) && method == "POST") ||
                   (path.Equals("/api/users", StringComparison.OrdinalIgnoreCase) && method == "POST") ||
                   (path.Equals("/register", StringComparison.OrdinalIgnoreCase) && (method == "GET" || method == "POST")) ||
                   (path.Equals("/auth/register", StringComparison.OrdinalIgnoreCase) && (method == "GET" || method == "POST")) ||
                   ((path.Equals("/users", StringComparison.OrdinalIgnoreCase) ||
                     path.Equals("/user", StringComparison.OrdinalIgnoreCase) ||
                     path.Equals("/users/create", StringComparison.OrdinalIgnoreCase) ||
                     path.Equals("/user/create", StringComparison.OrdinalIgnoreCase)) && method == "POST");
        }

        private static bool IsPublicJobReadRoute(string path, string method)
        {
            if (method != "GET") return false;

            return path.Equals("/api/jobs", StringComparison.OrdinalIgnoreCase) ||
                   ApiJobsByIdRegex.IsMatch(path) ||
                   path.Equals("/jobs", StringComparison.OrdinalIgnoreCase) ||
                   path.Equals("/job-posts", StringComparison.OrdinalIgnoreCase) ||
                   MvcJobsDetailsRegex.IsMatch(path);
        }

        private static bool IsPublicAnnouncementReadRoute(string path, string method)
        {
            if (method != "GET") return false;

            return path.Equals("/api/announcements", StringComparison.OrdinalIgnoreCase) ||
                   ApiAnnouncementsByIdRegex.IsMatch(path) ||
                   path.Equals("/announcements", StringComparison.OrdinalIgnoreCase) ||
                   path.Equals("/announcement", StringComparison.OrdinalIgnoreCase) ||
                   MvcAnnouncementsDetailsRegex.IsMatch(path);
        }

        private static bool IsMutatingMethod(string method)
        {
            return method is "POST" or "PUT" or "PATCH" or "DELETE";
        }

        private async Task DenyUnauthorizedAsync(HttpContext context, bool isApi)
        {
            var path = context.Request.Path.Value ?? "/";
            var method = context.Request.Method;

            _logger.LogWarning("⛔ AuthMiddleware [401]: Kimlik doğrulaması gerekli -> {Method} {Path}", method, path);

            if (isApi)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "login required" }));
                return;
            }

            var returnUrl = Uri.EscapeDataString(context.Request.Path + context.Request.QueryString);
            context.Response.Redirect($"/login?returnUrl={returnUrl}");
        }

        private async Task DenyForbiddenAsync(HttpContext context, bool isApi, string reason)
        {
            var path = context.Request.Path.Value ?? "/";
            var method = context.Request.Method;

            _logger.LogWarning("🚫 AuthMiddleware [403]: Yetkisiz işlem engellendi -> {Method} {Path} | Sebep: {Reason}",
                method, path, reason);

            context.Response.StatusCode = StatusCodes.Status403Forbidden;

            if (isApi)
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "not allowed" }));
                return;
            }

            context.Response.ContentType = "text/html; charset=utf-8";
            var safeReason = System.Net.WebUtility.HtmlEncode(reason);
            await context.Response.WriteAsync($$"""
                <!DOCTYPE html>
                <html lang="tr">
                <head>
                    <meta charset="UTF-8">
                    <meta name="viewport" content="width=device-width, initial-scale=1.0">
                    <title>403 Yetkisiz Erişim — Alumni Tracking System</title>
                    <style>
                        body { font-family: 'Inter', system-ui, sans-serif; background:#F8FAFC; color:#1E293B; margin:0; display:flex; align-items:center; justify-content:center; min-height:100vh; padding:24px; }
                        .card { max-width:520px; width:100%; background:#fff; border:1px solid #E2E8F0; border-top:4px solid #DC2626; border-radius:12px; padding:32px; box-shadow:0 10px 25px rgba(15,23,42,.08); }
                        .code { display:inline-block; background:#FEF2F2; color:#DC2626; font-weight:700; font-size:.82rem; padding:4px 10px; border-radius:999px; margin-bottom:12px; }
                        h1 { margin:0 0 10px; font-size:1.5rem; color:#1E3A8A; }
                        p { margin:0 0 22px; color:#475569; line-height:1.6; }
                        .actions { display:flex; gap:10px; flex-wrap:wrap; }
                        a { display:inline-block; padding:10px 16px; border-radius:8px; text-decoration:none; font-weight:600; font-size:.875rem; }
                        .btn-navy { background:#1E3A8A; color:#fff; }
                        .btn-outline { border:1px solid #CBD5E1; color:#1E293B; }
                    </style>
                </head>
                <body>
                    <div class="card">
                        <span class="code">403 Forbidden</span>
                        <h1>Bu işlem için yetkiniz yok</h1>
                        <p>{{safeReason}}</p>
                        <div class="actions">
                            <a href="/" class="btn-navy">Ana Sayfaya Dön</a>
                            <a href="/jobs" class="btn-outline">İş İlanları</a>
                        </div>
                    </div>
                </body>
                </html>
                """);
        }
    }

    public static class AuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthMiddleware>();
        }
    }
}
