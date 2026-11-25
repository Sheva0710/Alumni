using Alumni.API.Common;
using Alumni.Application.DTOs.Applications;
using Alumni.Application.DTOs.Jobs;
using Alumni.Application.DTOs.Users;
using Alumni.Application.Interfaces;
using Alumni.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Alumni.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Tags("Users")]
    public class ApiUserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJobPostService _jobPostService;
        private readonly IApplicationService _applicationService;

        public ApiUserController(
            IUserService userService,
            IJobPostService jobPostService,
            IApplicationService applicationService)
        {
            _userService = userService;
            _jobPostService = jobPostService;
            _applicationService = applicationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllAsync();
            var safeUsers = users.Select(u => UserResponseDto.FromEntity(u, includeJobs: false)).ToList();
            return Ok(safeUsers);
        }

        /// <summary>
        /// GET /api/users/{id}
        /// Include / Eager Loading kuralı: Kullanıcı detayını ve varsa açtığı iş ilanlarını (jobs: [...]) tek yanıtta döner.
        /// İlanı yoksa bile jobs: [] boş dizisi döner. passwordHash asla döndürülmez.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound(new { message = $"ID {id} olan kullanıcı bulunamadı." });

            return Ok(UserResponseDto.FromEntity(user, includeJobs: true));
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.Email))
                return BadRequest(new { error = "Ad ve e-posta zorunludur." });

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            if (await _userService.GetByEmailAsync(normalizedEmail) != null)
                return Conflict(new { error = "Bu e-posta adresiyle kayıtlı bir hesap zaten mevcut." });

            var plainPassword = string.IsNullOrWhiteSpace(request.Password) ? "secret123" : request.Password;
            if (plainPassword.Length < 6)
                return BadRequest(new { error = "Şifre en az 6 karakter olmalıdır." });

            var assignedRole = "Alumni";
            if (User.IsAdminRole() &&
                (string.Equals(request.Role, "Admin", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(request.Role, "Alumni", StringComparison.OrdinalIgnoreCase)))
            {
                assignedRole = string.Equals(request.Role, "Admin", StringComparison.OrdinalIgnoreCase) ? "Admin" : "Alumni";
            }

            var newUser = new User
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName?.Trim() ?? string.Empty,
                Email = normalizedEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword, workFactor: 12),
                // DATA_DESIGN.md: Açık kayıtta rol her zaman Alumni (graduate) olarak atanır; yalnızca Admin rol belirleyebilir.
                Role = assignedRole
            };

            await _userService.AddAsync(newUser);
            return Created($"/api/users/{newUser.Id}", UserResponseDto.FromEntity(newUser, includeJobs: false));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            // Güvenlik: Alumni kendi profilini güncellerken kendi rolünü Admin'e yükseltemez
            var safeRole = User.IsAdminRole() ? request.Role : null;
            var updated = await _userService.UpdateAsync(id, request.FirstName, request.LastName, request.Email, safeRole);
            if (!updated) return NotFound(new { message = $"ID {id} olan kullanıcı bulunamadı." });

            var user = await _userService.GetByIdAsync(id);
            return Ok(UserResponseDto.FromEntity(user!, includeJobs: true));
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchUser(int id, [FromBody] PatchUserRequest request)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound(new { message = $"ID {id} olan kullanıcı bulunamadı." });

            // Güvenlik: Alumni kendi profilini güncellerken rol yükseltemez
            var safeRole = User.IsAdminRole() ? request.Role : null;
            await _userService.PatchAsync(id, request.FirstName, request.LastName, request.Email, safeRole);
            var updatedUser = await _userService.GetByIdAsync(id);
            return Ok(UserResponseDto.FromEntity(updatedUser!, includeJobs: true));
        }

        /// <summary>
        /// DELETE /api/users/{id}
        /// DATA_DESIGN.md Silme Kuralı: Restrict.
        /// Kullanıcının yayında olan iş ilanları varsa silme engellenir ve 409 Conflict döner.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserWithRuleAsync(id);

            return result switch
            {
                DeleteUserResult.NotFound => NotFound(new { message = $"ID {id} olan kullanıcı bulunamadı." }),
                DeleteUserResult.HasJobPosts => Conflict(new { message = "Kullanıcıya ait yayında olan iş ilanları bulunmaktadır. DATA_DESIGN.md Restrict kuralı gereği kullanıcı silinemez. Önce ilanları kaldırınız." }),
                DeleteUserResult.Success => NoContent(),
                _ => BadRequest()
            };
        }

        // =========================================================================
        // NESTED ROUTES (İç İçe Yönlendirmeler)
        // =========================================================================

        /// <summary>
        /// POST /api/users/{id}/jobs
        /// Nested Route: Kullanıcı adına yeni bir iş ilanı açar.
        /// İlan sahibinin ID'si URL'den (:id) alınır, body'den değil.
        /// Kullanıcı bulunamazsa 404 Not Found döner.
        /// </summary>
        [HttpPost("{id}/jobs")]
        public async Task<IActionResult> CreateJobForUser(int id, [FromBody] CreateJobPostRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Company))
            {
                return BadRequest(new { message = "İlan başlığı (title) ve şirket adı (company) zorunludur." });
            }

            if (request.Title.Trim().Length > 120)
            {
                return BadRequest(new { message = "İlan başlığı (title) en fazla 120 karakter olabilir." });
            }

            var jobPost = await _jobPostService.CreateForUserAsync(
                id,
                request.Title,
                request.Company,
                request.City,
                request.Description
            );

            if (jobPost == null)
            {
                return NotFound(new { message = $"ID {id} olan kullanıcı bulunamadı." });
            }

            return Created($"/api/jobs/{jobPost.Id}", jobPost);
        }

        /// <summary>
        /// GET /api/users/{id}/jobs
        /// Nested Route: Belirli bir kullanıcının açtığı iş ilanlarını listeler.
        /// Kullanıcı bulunamazsa 404 Not Found döner.
        /// </summary>
        [HttpGet("{id}/jobs")]
        public async Task<IActionResult> GetJobsByUser(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = $"ID {id} olan kullanıcı bulunamadı." });
            }

            var jobs = await _jobPostService.GetByUserIdAsync(id);
            return Ok(jobs);
        }

        /// <summary>
        /// GET /api/users/{id}/applications
        /// Nested Route: Bir kullanıcının başvurduğu tüm iş ilanlarını listeler.
        /// Kullanıcı bulunamazsa 404 Not Found döner.
        /// </summary>
        [HttpGet("{id}/applications")]
        public async Task<IActionResult> GetApplicationsByUser(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = $"ID {id} olan kullanıcı bulunamadı." });
            }

            var applications = await _applicationService.GetApplicationsByUserIdAsync(id);
            return Ok(applications);
        }
    }
}
