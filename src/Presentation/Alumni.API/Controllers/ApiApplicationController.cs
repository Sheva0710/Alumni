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

        public ApiApplicationController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        /// <summary>
        /// GET /api/applications
        /// Doğrudan Rota: Sistemdeki tüm başvuruları listeler.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllApplications()
        {
            var applications = await _applicationService.GetAllAsync();
            return Ok(applications);
        }

        /// <summary>
        /// GET /api/applications/{id}
        /// Doğrudan Rota: Belirli bir başvurunun detayını getirir.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetApplicationById(int id)
        {
            var application = await _applicationService.GetByIdAsync(id);
            if (application == null)
            {
                return NotFound(new { message = $"ID {id} olan başvuru bulunamadı." });
            }

            return Ok(application);
        }

        /// <summary>
        /// POST /api/applications
        /// Doğrudan Rota: userId ve jobPostId ile başvuru oluşturur.
        /// - 201 Created: Başarılı başvuru.
        /// - 404 Not Found: İlan veya kullanıcı bulunamadı.
        /// - 409 Conflict: Mükerrer başvuru (UNIQUE user_id, job_post_id).
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateApplicationDirect([FromBody] DirectCreateApplicationRequest request)
        {
            if (request == null || request.UserId <= 0 || request.JobPostId <= 0)
            {
                return BadRequest(new { message = "Geçerli bir userId ve jobPostId belirtilmelidir." });
            }

            var result = await _applicationService.ApplyAsync(request.JobPostId, request.UserId);

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
        /// Doğrudan Rota: Başvurunun durumunu (Applied, Reviewing, Interview, Accepted, Rejected) günceller.
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
        /// Doğrudan Rota: Bir başvuruyu iptal eder / siler.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplication(int id)
        {
            var deleted = await _applicationService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound(new { message = $"ID {id} olan başvuru bulunamadı." });
            }

            return NoContent();
        }
    }
}
