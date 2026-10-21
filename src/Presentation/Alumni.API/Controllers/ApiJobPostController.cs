using Alumni.Application.DTOs.Applications;
using Alumni.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Alumni.API.Controllers
{
    [ApiController]
    [Route("api/jobs")]
    [Tags("Job Postings & Applications")]
    public class ApiJobPostController : ControllerBase
    {
        private readonly IJobPostService _jobPostService;
        private readonly IApplicationService _applicationService;

        public ApiJobPostController(
            IJobPostService jobPostService,
            IApplicationService applicationService)
        {
            _jobPostService = jobPostService;
            _applicationService = applicationService;
        }

        /// <summary>
        /// GET /api/jobs
        /// Tüm iş ilanlarını listeler.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllJobs()
        {
            var jobs = await _jobPostService.GetAllAsync();
            return Ok(jobs);
        }

        /// <summary>
        /// GET /api/jobs/{id}
        /// Tek bir iş ilanının detayını getirir.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobById(int id)
        {
            var job = await _jobPostService.GetByIdAsync(id);
            if (job == null)
            {
                return NotFound(new { message = $"ID {id} olan iş ilanı bulunamadı." });
            }

            return Ok(job);
        }

        /// <summary>
        /// DELETE /api/jobs/{id}
        /// Bir iş ilanını siler. Cascade silme kuralı gereği bu ilana yapılan başvurular da otomatik silinir.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var deleted = await _jobPostService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound(new { message = $"ID {id} olan iş ilanı bulunamadı." });
            }

            return NoContent();
        }

        /// <summary>
        /// POST /api/jobs/{id}/applications
        /// Nested Route: Belirli bir iş ilanına başvuru yapar.
        /// - 201 Created: Başvuru başarılı.
        /// - 404 Not Found: İlan veya kullanıcı veritabanında yok.
        /// - 409 Conflict: Mükerrer başvuru (UNIQUE user_id, job_post_id kısıtı).
        /// </summary>
        [HttpPost("{id}/applications")]
        public async Task<IActionResult> ApplyToJob(int id, [FromBody] CreateApplicationRequest request)
        {
            if (request == null || request.UserId <= 0)
            {
                return BadRequest(new { message = "Geçerli bir kullanıcı ID'si (userId) belirtilmelidir." });
            }

            var result = await _applicationService.ApplyAsync(id, request.UserId);

            return result.Status switch
            {
                ApplyResultStatus.JobNotFound => NotFound(new { message = result.Message }),
                ApplyResultStatus.UserNotFound => NotFound(new { message = result.Message }),
                ApplyResultStatus.AlreadyApplied => Conflict(new { message = result.Message }),
                ApplyResultStatus.Success => Created($"/api/jobs/{id}/applications/{result.Application!.Id}", result.Application),
                _ => BadRequest()
            };
        }

        /// <summary>
        /// GET /api/jobs/{id}/applications
        /// Nested Route: Bir iş ilanına başvuran adayların listesini döner.
        /// İlan bulunamazsa 404 Not Found döner.
        /// </summary>
        [HttpGet("{id}/applications")]
        public async Task<IActionResult> GetApplicantsByJob(int id)
        {
            var job = await _jobPostService.GetByIdAsync(id);
            if (job == null)
            {
                return NotFound(new { message = $"ID {id} olan iş ilanı bulunamadı." });
            }

            var applications = await _applicationService.GetApplicantsByJobIdAsync(id);
            return Ok(applications);
        }
    }
}
