using System.Security.Claims;

namespace Alumni.API.Common
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool IsAuthenticatedUser(this ClaimsPrincipal? user)
        {
            return user?.Identity?.IsAuthenticated == true && user.GetUserId().HasValue;
        }

        public static int? GetUserId(this ClaimsPrincipal? user)
        {
            var raw = user?.FindFirst("id")?.Value
                      ?? user?.FindFirst("sub")?.Value
                      ?? user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(raw, out var id) ? id : null;
        }

        public static string GetUserRole(this ClaimsPrincipal? user)
        {
            return user?.FindFirst("role")?.Value
                   ?? user?.FindFirst(ClaimTypes.Role)?.Value
                   ?? string.Empty;
        }

        public static string GetUserName(this ClaimsPrincipal? user)
        {
            return user?.FindFirst("name")?.Value
                   ?? user?.Identity?.Name
                   ?? string.Empty;
        }

        public static bool IsAdminRole(this ClaimsPrincipal? user)
        {
            return string.Equals(user.GetUserRole(), "Admin", StringComparison.OrdinalIgnoreCase);
        }
    }
}
