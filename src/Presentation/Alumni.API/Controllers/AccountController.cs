using Alumni.API.Common;
using Alumni.Application.DTOs.Users;
using Alumni.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Alumni.API.Controllers
{
    /// <summary>
    /// Oturum açmış kullanıcıya özel kişisel sayfalar:
    /// - /profile  (Kişisel Mezun Profili, İlanlarım, Başvurularım ve Profil Güncelleme)
    /// - /settings (Hesap Bilgileri, Şifre Değiştirme ve Oturum Güvenliği)
    /// Tüm işlemler daima JWT'deki oturum açmış kullanıcı (self) üzerinden yürütülür.
    /// </summary>
    [Tags("User Personal Pages (MVC)")]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IApplicationService _applicationService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IConfiguration _configuration;

        public AccountController(
            IUserService userService,
            IApplicationService applicationService,
            IJwtTokenService jwtTokenService,
            IConfiguration configuration)
        {
            _userService = userService;
            _applicationService = applicationService;
            _jwtTokenService = jwtTokenService;
            _configuration = configuration;
        }

        [HttpGet("/profile")]
        public async Task<IActionResult> Profile()
        {
            var currentUserId = User.GetUserId();
            if (!currentUserId.HasValue)
            {
                return Redirect("/login?returnUrl=/profile");
            }

            var user = await _userService.GetByIdAsync(currentUserId.Value);
            if (user == null)
            {
                return Redirect("/logout");
            }

            var applications = await _applicationService.GetApplicationsByUserIdAsync(user.Id);

            ViewData["Title"] = "Profilim";
            ViewData["ActiveNav"] = "/profile";

            var model = new UserProfileViewModel
            {
                User = user,
                JobPosts = user.JobPosts?.OrderByDescending(j => j.CreatedAt).ToList() ?? new(),
                Applications = applications
            };

            return View(model);
        }

        [HttpPost("/profile")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileRequest request)
        {
            var currentUserId = User.GetUserId();
            if (!currentUserId.HasValue)
            {
                return Redirect("/login?returnUrl=/profile");
            }

            var result = await _userService.UpsertProfileAsync(currentUserId.Value, request);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return Redirect("/profile");
        }

        [HttpGet("/settings")]
        public async Task<IActionResult> Settings()
        {
            var currentUserId = User.GetUserId();
            if (!currentUserId.HasValue)
            {
                return Redirect("/login?returnUrl=/settings");
            }

            var user = await _userService.GetByIdAsync(currentUserId.Value);
            if (user == null)
            {
                return Redirect("/logout");
            }

            ViewData["Title"] = "Hesap Ayarları";
            ViewData["ActiveNav"] = "/settings";

            return View(user);
        }

        [HttpPost("/settings/account")]
        public async Task<IActionResult> UpdateAccount([FromForm] UpdateAccountRequest request)
        {
            var currentUserId = User.GetUserId();
            if (!currentUserId.HasValue)
            {
                return Redirect("/login?returnUrl=/settings");
            }

            var result = await _userService.UpdateOwnAccountAsync(currentUserId.Value, request);
            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = result.Message;
                return Redirect("/settings");
            }

            // Ad/soyad veya e-posta değiştiğinde üst menünün anında güncellenmesi için JWT çerezini yenile
            if (result.User != null)
            {
                var freshToken = _jwtTokenService.GenerateToken(result.User);
                var expirationMinutes = _configuration.GetValue<int>("Jwt:ExpirationMinutes", 120);
                Response.Cookies.Append(JwtTokenService.AuthCookieName, freshToken, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax,
                    Secure = Request.IsHttps,
                    Path = "/",
                    Expires = DateTimeOffset.UtcNow.AddMinutes(expirationMinutes)
                });
            }

            TempData["SuccessMessage"] = result.Message;
            return Redirect("/settings");
        }

        [HttpPost("/settings/password")]
        public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordRequest request)
        {
            var currentUserId = User.GetUserId();
            if (!currentUserId.HasValue)
            {
                return Redirect("/login?returnUrl=/settings");
            }

            var result = await _userService.ChangePasswordAsync(currentUserId.Value, request);
            if (result.Succeeded)
            {
                TempData["PasswordSuccessMessage"] = result.Message;
            }
            else
            {
                TempData["PasswordErrorMessage"] = result.Message;
            }

            return Redirect("/settings#security");
        }
    }
}
