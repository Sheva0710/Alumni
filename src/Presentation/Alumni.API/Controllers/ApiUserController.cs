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
            return Ok(users);
        }

        /// <summary>
        /// GET /api/users/{id}
        /// Include / Eager Loading kuralı: Kullanıcı detayını ve varsa açtığı iş ilanlarını (jobs: [...]) tek yanıtta döner.
        /// İlanı yoksa bile jobs: [] boş dizisi döner.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound(new { message = $"ID {id} olan kullanıcı bulunamadı." });

            return Ok(new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Role,
                user.CreatedAt,
                user.Profile,
                Jobs = user.JobPosts ?? new List<JobPost>()
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.Email))
                return BadRequest("Ad ve e-posta zorunludur.");

            var newUser = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName ?? string.Empty,
                Email = request.Email,
                Role = request.Role ?? "Alumni"
            };

            await _userService.AddAsync(newUser);
            return Created($"/api/users/{newUser.Id}", newUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            var updated = await _userService.UpdateAsync(id, request.FirstName, request.LastName, request.Email, request.Role);
            if (!updated) return NotFound(new { message = $"ID {id} olan kullanıcı bulunamadı." });

            return Ok(await _userService.GetByIdAsync(id));
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchUser(int id, [FromBody] PatchUserRequest request)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound(new { message = $"ID {id} olan kullanıcı bulunamadı." });

            await _userService.PatchAsync(id, request.FirstName, request.LastName, request.Email, request.Role);
            return Ok(await _userService.GetByIdAsync(id));
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

            var jobPost = await _jobPostService.CreateForUserAsync(
                id,
                request.Title,
                request.Company,
                request.Description ?? string.Empty
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
