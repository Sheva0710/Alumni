using Alumni.Application.DTOs.Announcements;
using Alumni.Application.Interfaces;
using Alumni.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Alumni.API.Controllers
{
    [ApiController]
    [Route("api/announcements")]
    [Tags("Announcements")]
    public class ApiAnnouncementController : ControllerBase
    {
        private readonly IAnnouncementService _announcementService;

        public ApiAnnouncementController(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var announcements = await _announcementService.GetAllAsync();
            return Ok(announcements);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var announcement = await _announcementService.GetByIdAsync(id);
            if (announcement == null)
                return NotFound(new { message = $"ID {id} olan duyuru bulunamadı." });

            return Ok(announcement);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAnnouncementRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Content))
                return BadRequest("Başlık ve içerik zorunludur.");

            var newAnnouncement = new Announcement
            {
                Title = request.Title,
                Content = request.Content,
                Author = string.IsNullOrWhiteSpace(request.Author) ? "Kariyer Merkezi" : request.Author
            };

            var created = await _announcementService.AddAsync(newAnnouncement);
            return Created($"/api/announcements/{created.Id}", created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAnnouncementRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Content))
                return BadRequest("Başlık ve içerik zorunludur.");

            var updated = await _announcementService.UpdateAsync(id, request.Title, request.Content, request.Author, request.IsActive);
            if (!updated)
                return NotFound(new { message = $"ID {id} olan duyuru bulunamadı." });

            return Ok(await _announcementService.GetByIdAsync(id));
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] PatchAnnouncementRequest request)
        {
            var announcement = await _announcementService.GetByIdAsync(id);
            if (announcement == null)
                return NotFound(new { message = $"ID {id} olan duyuru bulunamadı." });

            var updated = await _announcementService.PatchAsync(id, request.Title, request.Content, request.Author, request.IsActive);
            if (!updated)
                return NotFound(new { message = $"ID {id} olan duyuru bulunamadı." });

            return Ok(await _announcementService.GetByIdAsync(id));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _announcementService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { message = $"ID {id} olan duyuru bulunamadı." });

            return NoContent();
        }
    }
}
