using Alumni.API.Common;
using Alumni.Application.DTOs.Applications;
using Alumni.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
        /// Tüm iş ilanlarını listeler (guest, graduate, admin: yes).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllJobs()
        {
            var jobs = await _jobPostService.GetAllAsync();
            return Ok(jobs);
        }

        /// <summary>
        /// GET /api/jobs/{id}
        /// Tek bir iş ilanının detayını getirir (guest, graduate, admin: yes).
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
        /// Bir iş ilanını siler. Yalnızca ilan sahibi (own) veya Admin silebilir.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var job = await _jobPostService.GetByIdAsync(id);
            if (job == null)
            {
                return NotFound(new { message = $"ID {id} olan iş ilanı bulunamadı." });
            }

            var currentUserId = User.GetUserId();
            if (!User.IsAdminRole() && job.PostedByUserId != currentUserId)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { error = "not allowed" });
            }

            await _jobPostService.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// POST /api/jobs/{id}/applications
        /// Nested Route: Oturum açmış mezun kendi adına (self) iş ilanına başvurur.
        /// Güvenlik (9. Hafta): Gövdede "userId": B gönderilse bile YOK SAYILIR; başvuran daima oturumdaki kullanıcıdır.
        /// Admin rolü başvuru yapamaz (403 not allowed).
        /// </summary>
        [HttpPost("{id}/applications")]
        public async Task<IActionResult> ApplyToJob(
            int id,
            [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] CreateApplicationRequest? request = null)
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

            // İlan varlık & kendi ilanına başvuramama (Self-Application Invariant) kontrolü
            var job = await _jobPostService.GetByIdAsync(id);
            if (job == null)
            {
                return NotFound(new { message = $"ID {id} olan iş ilanı bulunamadı." });
            }

            if (job.PostedByUserId == currentUserId.Value)
            {
                return BadRequest(new { error = "Kendi yayınladığınız iş ilanına başvuru yapamazsınız." });
            }

            // Gövdedeki request.UserId kasıtlı olarak YOK SAYILIR; başvuran daima oturumdaki kullanıcıdır (self).
            var result = await _applicationService.ApplyAsync(id, currentUserId.Value);

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
        /// Nested Route: Bir iş ilanına başvuran adayların listesini döner (yalnızca ilan sahibi veya Admin).
        /// </summary>
        [HttpGet("{id}/applications")]
        public async Task<IActionResult> GetApplicantsByJob(int id)
        {
            var job = await _jobPostService.GetByIdAsync(id);
            if (job == null)
            {
                return NotFound(new { message = $"ID {id} olan iş ilanı bulunamadı." });
            }

            var currentUserId = User.GetUserId();
            if (!User.IsAdminRole() && job.PostedByUserId != currentUserId)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { error = "not allowed" });
            }

            var applications = await _applicationService.GetApplicantsByJobIdAsync(id);
            return Ok(applications);
        }
    }
}
