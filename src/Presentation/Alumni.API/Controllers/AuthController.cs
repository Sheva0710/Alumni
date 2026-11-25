using Alumni.API.Common;
using Alumni.Application.DTOs.Auth;
using Alumni.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Alumni.API.Controllers
{
    [Tags("Auth (MVC)")]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IConfiguration _configuration;

        public AuthController(
            IUserService userService,
            IJwtTokenService jwtTokenService,
            IConfiguration configuration)
        {
            _userService = userService;
            _jwtTokenService = jwtTokenService;
            _configuration = configuration;
        }

        [HttpGet("/register")]
        [HttpGet("/auth/register")]
        public IActionResult Register()
        {
            ViewData["Title"] = "Kayıt Ol";
            ViewData["ActiveNav"] = "/register";
            return View(new RegisterRequest());
        }

        [HttpPost("/register")]
        [HttpPost("/auth/register")]
        public async Task<IActionResult> Register([FromForm] RegisterRequest request)
        {
            ViewData["Title"] = "Kayıt Ol";
            ViewData["ActiveNav"] = "/register";

            var result = await _userService.RegisterAsync(request, requireConfirmPassword: true);

            if (result.Status != RegisterUserStatus.Success)
            {
                ViewData["ErrorMessage"] = result.ErrorMessage;
                // Güvenlik: hata durumunda şifre alanlarını geri doldurma
                request.Password = null;
                request.ConfirmPassword = null;
                return View(request);
            }

            ViewData["SuccessMessage"] = $"Hoş geldiniz, {result.User!.FirstName} {result.User.LastName}! Hesabınız '{result.User.Role}' rolüyle başarıyla oluşturuldu. Artık giriş yapabilirsiniz.";
            ViewData["CreatedUserId"] = result.User.Id;
            ViewData["CreatedUserEmail"] = result.User.Email;

            return View(new RegisterRequest());
        }

        [HttpGet("/login")]
        [HttpGet("/auth/login")]
        public IActionResult Login([FromQuery] string? returnUrl = null, [FromQuery] bool loggedOut = false)
        {
            ViewData["Title"] = "Giriş Yap";
            ViewData["ActiveNav"] = "/login";

            if (loggedOut)
            {
                ViewData["SuccessMessage"] = "Oturumunuz güvenli bir şekilde kapatıldı.";
            }

            return View(new LoginRequest { ReturnUrl = returnUrl });
        }

        [HttpPost("/login")]
        [HttpPost("/auth/login")]
        public async Task<IActionResult> Login([FromForm] LoginRequest request)
        {
            ViewData["Title"] = "Giriş Yap";
            ViewData["ActiveNav"] = "/login";

            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                ViewData["ErrorMessage"] = "Geçersiz e-posta veya şifre.";
                request.Password = null;
                return View(request);
            }

            var user = await _userService.AuthenticateAsync(request.Email, request.Password);
            if (user == null)
            {
                // Bilinmeyen e-posta ve yanlış şifre için aynı mesaj (enumeration koruması)
                ViewData["ErrorMessage"] = "Geçersiz e-posta veya şifre.";
                request.Password = null;
                return View(request);
            }

            var token = _jwtTokenService.GenerateToken(user);
            var expirationMinutes = _configuration.GetValue<int>("Jwt:ExpirationMinutes", 120);

            Response.Cookies.Append(JwtTokenService.AuthCookieName, token, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Secure = Request.IsHttps,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddMinutes(expirationMinutes)
            });

            if (!string.IsNullOrWhiteSpace(request.ReturnUrl) && Url.IsLocalUrl(request.ReturnUrl))
            {
                return Redirect(request.ReturnUrl);
            }

            return Redirect("/");
        }

        [HttpPost("/logout")]
        [HttpPost("/auth/logout")]
        [HttpGet("/logout")]
        [HttpGet("/auth/logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete(JwtTokenService.AuthCookieName, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Secure = Request.IsHttps,
                Path = "/"
            });

            return Redirect("/login?loggedOut=true");
        }
    }
}
