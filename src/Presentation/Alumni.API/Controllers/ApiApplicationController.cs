using Alumni.API.Common;
using Alumni.Application.DTOs.Applications;
using Alumni.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Alumni.API.Controllers
{
    [ApiController]
    [Route("api/applications")]
    [Tags("Applications")]
    public class ApiApplicationController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        private readonly IJobPostService _jobPostService;

        public ApiApplicationController(
            IApplicationService applicationService,
            IJobPostService jobPostService)
        {
            _applicationService = applicationService;
            _jobPostService = jobPostService;
        }

        /// <summary>
        /// GET /api/applications
        /// Admin tüm başvuruları, Alumni yalnızca kendi başvurularını görür.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllApplications()
        {
            var applications = await _applicationService.GetAllAsync();
            if (!User.IsAdminRole())
            {
                var currentUserId = User.GetUserId();
                applications = applications.Where(a => a.UserId == currentUserId).ToList();
            }

            return Ok(applications);
        }

        /// <summary>
        /// GET /api/applications/{id}
        /// Belirli bir başvurunun detayını getirir (yalnızca başvuran kullanıcı veya Admin).
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetApplicationById(int id)
        {
            var application = await _applicationService.GetByIdAsync(id);
            if (application == null)
            {
                return NotFound(new { message = $"ID {id} olan başvuru bulunamadı." });
            }

            if (!User.IsAdminRole() && application.UserId != User.GetUserId())
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { error = "not allowed" });
            }

            return Ok(application);
        }

        /// <summary>
        /// POST /api/applications
        /// Doğrudan Rota: Oturum açmış mezun kendi adına (self) başvuru oluşturur.
        /// Gövdedeki userId yok sayılır; başvuran daima oturumdaki kullanıcıdır.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateApplicationDirect([FromBody] DirectCreateApplicationRequest request)
        {
            var currentUserId = User.GetUserId();
            if (!currentUserId.HasValue)
            {
                return Unauthorized(new { error = "login required" });
            }

            if (User.IsAdminRole())
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { error = "not allowed" });
            }

            if (request == null || request.JobPostId <= 0)
            {
                return BadRequest(new { message = "Geçerli bir jobPostId belirtilmelidir." });
            }

            var job = await _jobPostService.GetByIdAsync(request.JobPostId);
            if (job == null)
            {
                return NotFound(new { message = $"ID {request.JobPostId} olan iş ilanı bulunamadı." });
            }

            if (job.PostedByUserId == currentUserId.Value)
            {
                return BadRequest(new { error = "Kendi yayınladığınız iş ilanına başvuru yapamazsınız." });
            }

            var result = await _applicationService.ApplyAsync(request.JobPostId, currentUserId.Value);

            return result.Status switch
            {
                ApplyResultStatus.JobNotFound => NotFound(new { message = result.Message }),
                ApplyResultStatus.UserNotFound => NotFound(new { message = result.Message }),
                ApplyResultStatus.AlreadyApplied => Conflict(new { message = result.Message }),
                ApplyResultStatus.Success => Created($"/api/applications/{result.Application!.Id}", result.Application),
                _ => BadRequest()
            };
        }

        /// <summary>
        /// PATCH /api/applications/{id}/status
        /// Başvurunun durumunu (Applied, Reviewing, Interview, Accepted, Rejected) günceller (Admin).
        /// </summary>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateApplicationStatusRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Status))
            {
                return BadRequest(new { message = "Durum (status) değeri boş bırakılamaz." });
            }

            var updated = await _applicationService.UpdateStatusAsync(id, request.Status.Trim());
            if (!updated)
            {
                return NotFound(new { message = $"ID {id} olan başvuru bulunamadı." });
            }

            return Ok(await _applicationService.GetByIdAsync(id));
        }

        /// <summary>
        /// DELETE /api/applications/{id}
        /// Bir başvuruyu iptal eder / siler (yalnızca başvuru sahibi veya Admin).
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplication(int id)
        {
            var application = await _applicationService.GetByIdAsync(id);
            if (application == null)
            {
                return NotFound(new { message = $"ID {id} olan başvuru bulunamadı." });
            }

            if (!User.IsAdminRole() && application.UserId != User.GetUserId())
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { error = "not allowed" });
            }

            await _applicationService.DeleteAsync(id);
            return NoContent();
        }
    }
}
