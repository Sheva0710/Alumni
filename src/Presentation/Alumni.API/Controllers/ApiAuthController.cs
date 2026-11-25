using Alumni.API.Common;
using Alumni.Application.DTOs.Auth;
using Alumni.Application.DTOs.Users;
using Alumni.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Alumni.API.Controllers
{
    [ApiController]
    [Route("api")]
    [Tags("Auth")]
    public class ApiAuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtTokenService _jwtTokenService;

        public ApiAuthController(IUserService userService, IJwtTokenService jwtTokenService)
        {
            _userService = userService;
            _jwtTokenService = jwtTokenService;
        }

        /// <summary>
        /// POST /api/register
        /// Yeni mezun hesabı oluşturur. Şifre BCrypt (hash + salt) ile saklanır.
        /// İstemci "role": "Admin" gönderse dahi yeni hesap her zaman "Alumni" rolüyle açılır.
        /// Yanıtta passwordHash asla döndürülmez.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { error = "Geçersiz kayıt isteği." });
            }

            var result = await _userService.RegisterAsync(request, requireConfirmPassword: false);

            return result.Status switch
            {
                RegisterUserStatus.ValidationError => BadRequest(new { error = result.ErrorMessage }),
                RegisterUserStatus.EmailAlreadyExists => Conflict(new { error = result.ErrorMessage }),
                RegisterUserStatus.Success => Created(
                    $"/api/users/{result.User!.Id}",
                    UserResponseDto.FromEntity(result.User!, includeJobs: false)),
                _ => BadRequest(new { error = "Kayıt işlemi tamamlanamadı." })
            };
        }

        /// <summary>
        /// POST /api/login
        /// E-posta ve şifreyi BCrypt ile doğrular; geçerliyse 200 { token } döner.
        /// Hatalı şifre veya bilinmeyen e-posta için aynı 401 { "error": "invalid email or password" } döner.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest? request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return Unauthorized(new { error = "invalid email or password" });
            }

            var user = await _userService.AuthenticateAsync(request.Email, request.Password);
            if (user == null)
            {
                return Unauthorized(new { error = "invalid email or password" });
            }

            var token = _jwtTokenService.GenerateToken(user);
            return Ok(new { token });
        }

        /// <summary>
        /// POST /api/logout
        /// Tarayıcı oturum çerezini (HttpOnly JWT cookie) temizler.
        /// </summary>
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete(JwtTokenService.AuthCookieName, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Secure = Request.IsHttps,
                Path = "/"
            });

            return Ok(new { message = "logged out" });
        }

        /// <summary>
        /// GET /api/me
        /// Geçerli JWT kimlik kartından oturum açmış kullanıcının { id, name, role } bilgilerini döner.
        /// Kart yoksa / geçersizse / süresi dolmuşsa 401 { "error": "login required" } döner.
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var idClaim = User.FindFirst("id")?.Value ?? User.FindFirst("sub")?.Value;
            var name = User.FindFirst("name")?.Value ?? User.Identity?.Name ?? string.Empty;
            var role = User.FindFirst("role")?.Value ?? string.Empty;

            if (!int.TryParse(idClaim, out var userId))
            {
                return Unauthorized(new { error = "login required" });
            }

            return Ok(new
            {
                id = userId,
                name,
                role
            });
        }
    }
}
