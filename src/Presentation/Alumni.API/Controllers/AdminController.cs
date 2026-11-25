using Alumni.API.Common;
using Alumni.Application.Interfaces;
using Alumni.Domain.Entities;
using Alumni.Persistence.Seed.Factories;
using Microsoft.AspNetCore.Mvc;

namespace Alumni.API.Controllers
{
    /// <summary>
    /// 9. Hafta Admin Paneli (MVC):
    /// Yalnızca 'Admin' rolündeki kullanıcılar erişebilir (AuthMiddleware tarafından korunur).
    /// Kullanıcılar, roller, iş ilanları, başvurular ve duyurular tek ekrandan yönetilir.
    /// </summary>
    [Route("admin")]
    [Tags("Admin Panel (MVC)")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IJobPostService _jobPostService;
        private readonly IApplicationService _applicationService;
        private readonly IAnnouncementService _announcementService;

        public AdminController(
            IUserService userService,
            IJobPostService jobPostService,
            IApplicationService applicationService,
            IAnnouncementService announcementService)
        {
            _userService = userService;
            _jobPostService = jobPostService;
            _applicationService = applicationService;
            _announcementService = announcementService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Admin Panel";
            ViewData["ActiveNav"] = "/admin";

            var model = new AdminDashboardViewModel
            {
                Users = await _userService.GetAllAsync(),
                JobPosts = await _jobPostService.GetAllAsync(),
                Applications = await _applicationService.GetAllAsync(),
                Announcements = await _announcementService.GetAllAsync()
            };

            return View(model);
        }

        // ---------------------------------------------------------------------
        // 1. KULLANICI & ROL YÖNETİMİ
        // ---------------------------------------------------------------------

        [HttpPost("users/create")]
        public async Task<IActionResult> CreateUser(
            [FromForm] string firstName,
            [FromForm] string lastName,
            [FromForm] string email,
            [FromForm] string password,
            [FromForm] string role)
        {
            if (string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                password.Length < 6)
            {
                TempData["ErrorMessage"] = "Ad, soyad, geçerli e-posta ve en az 6 karakterli şifre zorunludur.";
                return Redirect("/admin#users");
            }

            var normalizedEmail = email.Trim().ToLowerInvariant();
            if (await _userService.GetByEmailAsync(normalizedEmail) != null)
            {
                TempData["ErrorMessage"] = $"'{normalizedEmail}' e-posta adresi zaten kayıtlı.";
                return Redirect("/admin#users");
            }

            var safeRole = string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase) ? "Admin" : "Alumni";
            var user = new User
            {
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                Email = normalizedEmail,
                PasswordHash = UserFactory.HashPassword(password),
                Role = safeRole,
                CreatedAt = DateTime.UtcNow
            };

            await _userService.AddAsync(user);
            TempData["SuccessMessage"] = $"{user.FirstName} {user.LastName} ({user.Role}) başarıyla oluşturuldu.";
            return Redirect("/admin#users");
        }

        [HttpPost("users/{id}/role")]
        public async Task<IActionResult> ChangeUserRole(int id, [FromForm] string role)
        {
            var safeRole = string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase) ? "Admin" : "Alumni";
            var target = await _userService.GetByIdAsync(id);
            if (target == null)
            {
                TempData["ErrorMessage"] = $"ID {id} olan kullanıcı bulunamadı.";
                return Redirect("/admin#users");
            }

            await _userService.PatchAsync(id, null, null, null, safeRole);
            TempData["SuccessMessage"] = $"{target.FirstName} {target.LastName} kullanıcısının rolü '{safeRole}' olarak güncellendi. (Yeni rolün etkinleşmesi için kullanıcının yeniden giriş yapması gerekir.)";
            return Redirect("/admin#users");
        }

        [HttpPost("users/{id}/delete")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var currentUserId = User.GetUserId();
            if (currentUserId == id)
            {
                TempData["ErrorMessage"] = "Kendi aktif yönetici hesabınızı bu ekrandan silemezsiniz.";
                return Redirect("/admin#users");
            }

            var result = await _userService.DeleteUserWithRuleAsync(id);
            TempData[result == DeleteUserResult.Success ? "SuccessMessage" : "ErrorMessage"] = result switch
            {
                DeleteUserResult.Success => $"Kullanıcı #{id} başarıyla silindi.",
                DeleteUserResult.HasJobPosts => $"Kullanıcı #{id} silinemedi (Restrict Kuralı): Kullanıcının yayında olan iş ilanları var. Önce ilanları silin.",
                DeleteUserResult.NotFound => $"Kullanıcı #{id} bulunamadı.",
                _ => "Kullanıcı silinemedi."
            };

            return Redirect("/admin#users");
        }

        // ---------------------------------------------------------------------
        // 2. İŞ İLANI YÖNETİMİ
        // ---------------------------------------------------------------------

        [HttpPost("jobs/create")]
        public async Task<IActionResult> CreateJob(
            [FromForm] int postedByUserId,
            [FromForm] string title,
            [FromForm] string company,
            [FromForm] string? city,
            [FromForm] string? description)
        {
            if (postedByUserId <= 0 || string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(company))
            {
                TempData["ErrorMessage"] = "İlan sahibi, başlık ve şirket adı zorunludur.";
                return Redirect("/admin#jobs");
            }

            if (title.Trim().Length > 120)
            {
                TempData["ErrorMessage"] = "İlan başlığı en fazla 120 karakter olabilir.";
                return Redirect("/admin#jobs");
            }

            var created = await _jobPostService.CreateForUserAsync(postedByUserId, title.Trim(), company.Trim(), city?.Trim(), description?.Trim());
            if (created == null)
            {
                TempData["ErrorMessage"] = "Seçilen ilan sahibi kullanıcı bulunamadı.";
            }
            else
            {
                TempData["SuccessMessage"] = $"'{created.Title}' iş ilanı başarıyla yayınlandı.";
            }

            return Redirect("/admin#jobs");
        }

        [HttpPost("jobs/{id}/delete")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var deleted = await _jobPostService.DeleteAsync(id);
            if (deleted)
                TempData["SuccessMessage"] = $"İş ilanı #{id} ve bağlı başvuruları silindi (Cascade).";
            else
                TempData["ErrorMessage"] = $"İş ilanı #{id} bulunamadı.";

            return Redirect("/admin#jobs");
        }

        // ---------------------------------------------------------------------
        // 3. BAŞVURU YÖNETİMİ
        // ---------------------------------------------------------------------

        [HttpPost("applications/{id}/status")]
        public async Task<IActionResult> UpdateApplicationStatus(int id, [FromForm] string status)
        {
            var allowed = new[] { "Applied", "Reviewing", "Interview", "Accepted", "Rejected" };
            if (string.IsNullOrWhiteSpace(status) || !allowed.Contains(status.Trim()))
            {
                TempData["ErrorMessage"] = "Geçersiz başvuru durumu seçildi.";
                return Redirect("/admin#applications");
            }

            var updated = await _applicationService.UpdateStatusAsync(id, status.Trim());
            if (updated)
                TempData["SuccessMessage"] = $"Başvuru #{id} durumu '{status.Trim()}' olarak güncellendi.";
            else
                TempData["ErrorMessage"] = $"Başvuru #{id} bulunamadı.";

            return Redirect("/admin#applications");
        }

        [HttpPost("applications/{id}/delete")]
        public async Task<IActionResult> DeleteApplication(int id)
        {
            var deleted = await _applicationService.DeleteAsync(id);
            if (deleted)
                TempData["SuccessMessage"] = $"Başvuru #{id} silindi.";
            else
                TempData["ErrorMessage"] = $"Başvuru #{id} bulunamadı.";

            return Redirect("/admin#applications");
        }

        // ---------------------------------------------------------------------
        // 4. DUYURU YÖNETİMİ
        // ---------------------------------------------------------------------

        [HttpPost("announcements/create")]
        public async Task<IActionResult> CreateAnnouncement(
            [FromForm] string title,
            [FromForm] string content,
            [FromForm] string? author)
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
            {
                TempData["ErrorMessage"] = "Duyuru başlığı ve içeriği zorunludur.";
                return Redirect("/admin#announcements");
            }

            var announcement = new Announcement
            {
                Title = title.Trim(),
                Content = content.Trim(),
                Author = string.IsNullOrWhiteSpace(author) ? (User.GetUserName() is { Length: > 0 } n ? n : "Sistem Yöneticisi") : author.Trim(),
                IsActive = true
            };

            await _announcementService.AddAsync(announcement);
            TempData["SuccessMessage"] = $"'{announcement.Title}' duyurusu yayınlandı.";
            return Redirect("/admin#announcements");
        }

        [HttpPost("announcements/{id}/toggle")]
        public async Task<IActionResult> ToggleAnnouncement(int id)
        {
            var existing = await _announcementService.GetByIdAsync(id);
            if (existing == null)
            {
                TempData["ErrorMessage"] = $"Duyuru #{id} bulunamadı.";
                return Redirect("/admin#announcements");
            }

            var newState = !existing.IsActive;
            await _announcementService.PatchAsync(id, null, null, null, newState);
            TempData["SuccessMessage"] = $"Duyuru #{id} durumu '{(newState ? "Aktif" : "Pasif")}' olarak güncellendi.";
            return Redirect("/admin#announcements");
        }

        [HttpPost("announcements/{id}/delete")]
        public async Task<IActionResult> DeleteAnnouncement(int id)
        {
            var deleted = await _announcementService.DeleteAsync(id);
            if (deleted)
                TempData["SuccessMessage"] = $"Duyuru #{id} silindi.";
            else
                TempData["ErrorMessage"] = $"Duyuru #{id} bulunamadı.";

            return Redirect("/admin#announcements");
        }
    }
}
