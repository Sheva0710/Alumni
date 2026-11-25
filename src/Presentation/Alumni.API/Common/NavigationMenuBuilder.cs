using System.Security.Claims;

namespace Alumni.API.Common
{
    public sealed record NavMenuItem(
        string Label,
        string Href,
        bool IsAccent = false);

    public sealed record RoleNavigationModel(
        string RoleKey,
        string RoleLabel,
        bool IsAuthenticated,
        bool IsAdmin,
        int? UserId,
        string UserName,
        IReadOnlyList<NavMenuItem> MainNavItems,
        IReadOnlyList<NavMenuItem> FooterItems);

    /// <summary>
    /// Kullanıcı rolüne (Guest, Alumni, Admin) göre üst menü, alt menü ve oturum aksiyonlarını
    /// DATA_DESIGN.md §4 (Who May Do What) matrisine uygun olarak dinamik üreten tek merkezli yardımcı.
    /// Not: Menü gizleme yalnızca UX içindir; gerçek yetki kontrolü AuthMiddleware tarafından yapılır.
    /// </summary>
    public static class NavigationMenuBuilder
    {
        public static RoleNavigationModel Build(ClaimsPrincipal? user)
        {
            var isAuthenticated = user.IsAuthenticatedUser();
            var userId = user.GetUserId();
            var rawRole = user.GetUserRole();
            var isAdmin = isAuthenticated && user.IsAdminRole();
            var userName = isAuthenticated ? user.GetUserName() : string.Empty;

            if (!isAuthenticated)
            {
                // GUEST: Yalnızca herkese açık sayfalar (Mezunlar, Başvurular, Admin Panel gizli)
                var mainItems = new List<NavMenuItem>
                {
                    new("Ana Sayfa", "/"),
                    new("İş İlanları", "/jobs"),
                    new("Duyurular", "/announcements"),
                    new("Hakkında", "/about")
                };

                var footerItems = new List<NavMenuItem>
                {
                    new("Hakkında", "/about"),
                    new("İş İlanları", "/jobs"),
                    new("Duyurular", "/announcements"),
                    new("Giriş Yap", "/login"),
                    new("Kayıt Ol", "/register"),
                    new("Swagger / API", "/api/swagger")
                };

                return new RoleNavigationModel(
                    RoleKey: "Guest",
                    RoleLabel: "Ziyaretçi",
                    IsAuthenticated: false,
                    IsAdmin: false,
                    UserId: null,
                    UserName: string.Empty,
                    MainNavItems: mainItems,
                    FooterItems: footerItems);
            }

            if (isAdmin)
            {
                // ADMIN: Tüm yönetim modülleri + Profilim + Ayarlar + Admin Panel
                var mainItems = new List<NavMenuItem>
                {
                    new("Ana Sayfa", "/"),
                    new("Mezunlar", "/users"),
                    new("İş İlanları", "/jobs"),
                    new("Başvurular", "/applications"),
                    new("Profilim", "/profile"),
                    new("Ayarlar", "/settings"),
                    new("Admin Panel", "/admin", IsAccent: true)
                };

                var footerItems = new List<NavMenuItem>
                {
                    new("Admin Panel", "/admin", IsAccent: true),
                    new("Profilim", "/profile"),
                    new("Ayarlar", "/settings"),
                    new("Mezunlar", "/users"),
                    new("İş İlanları", "/jobs"),
                    new("Başvurular", "/applications"),
                    new("Duyurular", "/announcements"),
                    new("Hakkında", "/about"),
                    new("Swagger / API", "/api/swagger")
                };

                return new RoleNavigationModel(
                    RoleKey: "Admin",
                    RoleLabel: "Admin",
                    IsAuthenticated: true,
                    IsAdmin: true,
                    UserId: userId,
                    UserName: userName,
                    MainNavItems: mainItems,
                    FooterItems: footerItems);
            }

            // ALUMNI (GRADUATE): Mezun rehberi, İş İlanları, Başvurularım, Profilim (/profile), Ayarlar (/settings)
            var alumniMainItems = new List<NavMenuItem>
            {
                new("Ana Sayfa", "/"),
                new("Mezunlar", "/users"),
                new("İş İlanları", "/jobs"),
                new("Başvurularım", "/applications"),
                new("Profilim", "/profile"),
                new("Ayarlar", "/settings")
            };

            var alumniFooterItems = new List<NavMenuItem>
            {
                new("Profilim", "/profile"),
                new("Ayarlar", "/settings"),
                new("Mezunlar", "/users"),
                new("İş İlanları", "/jobs"),
                new("Başvurularım", "/applications"),
                new("Duyurular", "/announcements"),
                new("Hakkında", "/about"),
                new("Swagger / API", "/api/swagger")
            };

            return new RoleNavigationModel(
                RoleKey: string.IsNullOrWhiteSpace(rawRole) ? "Alumni" : rawRole,
                RoleLabel: string.IsNullOrWhiteSpace(rawRole) ? "Alumni" : rawRole,
                IsAuthenticated: true,
                IsAdmin: false,
                UserId: userId,
                UserName: userName,
                MainNavItems: alumniMainItems,
                FooterItems: alumniFooterItems);
        }
    }
}
