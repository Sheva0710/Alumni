using Alumni.Application.DTOs.Users;
using Alumni.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Alumni.API.Controllers
{
    /// <summary>
    /// Admin Panel REST API uç noktaları (/api/admin/*).
    /// AuthMiddleware tarafından korunur: Guest -> 401, Alumni -> 403, Admin -> 200.
    /// </summary>
    [ApiController]
    [Route("api/admin")]
    [Tags("Admin")]
    public class ApiAdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJobPostService _jobPostService;
        private readonly IApplicationService _applicationService;
        private readonly IAnnouncementService _announcementService;

        public ApiAdminController(
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

        /// <summary>
        /// GET /api/admin/overview
        /// Yalnızca Admin rolü için sistem istatistiklerini ve hash içermeyen kullanıcı listesini döner.
        /// </summary>
        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview()
        {
            var users = await _userService.GetAllAsync();
            var jobs = await _jobPostService.GetAllAsync();
            var applications = await _applicationService.GetAllAsync();
            var announcements = await _announcementService.GetAllAsync();

            return Ok(new
            {
                counts = new
                {
                    users = users.Count,
                    admins = users.Count(u => string.Equals(u.Role, "Admin", StringComparison.OrdinalIgnoreCase)),
                    alumni = users.Count(u => !string.Equals(u.Role, "Admin", StringComparison.OrdinalIgnoreCase)),
                    jobs = jobs.Count,
                    applications = applications.Count,
                    announcements = announcements.Count
                },
                users = users.Select(u => UserResponseDto.FromEntity(u, includeJobs: false))
            });
        }

        /// <summary>
        /// PATCH /api/admin/users/{id}/role
        /// Yalnızca Admin rolü bir kullanıcının rolünü 'Admin' veya 'Alumni' olarak değiştirebilir.
        /// </summary>
        [HttpPatch("users/{id}/role")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] RoleUpdateRequest request)
        {
            if (request == null ||
                (!string.Equals(request.Role, "Admin", StringComparison.OrdinalIgnoreCase) &&
                 !string.Equals(request.Role, "Alumni", StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest(new { error = "Rol yalnızca 'Admin' veya 'Alumni' olabilir." });
            }

            var normalizedRole = string.Equals(request.Role, "Admin", StringComparison.OrdinalIgnoreCase) ? "Admin" : "Alumni";
            var updated = await _userService.PatchAsync(id, null, null, null, normalizedRole);
            if (!updated)
            {
                return NotFound(new { error = $"ID {id} olan kullanıcı bulunamadı." });
            }

            var user = await _userService.GetByIdAsync(id);
            return Ok(UserResponseDto.FromEntity(user!, includeJobs: false));
        }
    }

    public class RoleUpdateRequest
    {
        public string? Role { get; set; }
    }
}
