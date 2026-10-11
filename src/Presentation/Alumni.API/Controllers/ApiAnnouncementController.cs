using Alumni.Application.DTOs.Announcements;
using Alumni.Application.Interfaces;
using Alumni.Domain.Entities;
using Microsoft.AspNetCore.Http;
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
        public IActionResult GetAll()
        {
            return Ok(_announcementService.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var announcement = _announcementService.GetById(id);
            if (announcement == null)
                return NotFound(new { message = $"ID {id} olan duyuru bulunamadı." });

            return Ok(announcement);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateAnnouncementRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Content))
                return BadRequest("Başlık ve içerik zorunludur.");

            var newAnnouncement = new Announcement
            {
                Title = request.Title,
                Content = request.Content,
                Author = string.IsNullOrWhiteSpace(request.Author) ? "Kariyer Merkezi" : request.Author
            };

            var created = _announcementService.Add(newAnnouncement);
            return Created($"/api/announcements/{created.Id}", created);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateAnnouncementRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Content))
                return BadRequest("Başlık ve içerik zorunludur.");

            var updated = _announcementService.Update(id, request.Title, request.Content, request.Author, request.IsActive);
            if (!updated)
                return NotFound(new { message = $"ID {id} olan duyuru bulunamadı." });

            return Ok(_announcementService.GetById(id));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var deleted = _announcementService.Delete(id);
            if (!deleted)
                return NotFound(new { message = $"ID {id} olan duyuru bulunamadı." });

            return NoContent();
        }
    }
}
