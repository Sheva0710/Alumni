using Alumni.Application.DTOs.Announcements;
using Alumni.Application.Interfaces;
using Alumni.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Alumni.API.Controllers
{
    [Route("announcements")]
    [Route("announcement")]
    [Tags("Announcements (MVC)")]
    public class AnnouncementController : Controller
    {
        private readonly IAnnouncementService _announcementService;

        public AnnouncementController(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var announcements = await _announcementService.GetAllAsync();
            return View(announcements);
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var announcement = await _announcementService.GetByIdAsync(id);
            if (announcement == null) return NotFound();

            return View(announcement);
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var announcement = await _announcementService.GetByIdAsync(id);
            if (announcement == null) return NotFound();

            return View(announcement);
        }

        [HttpPost]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CreateAnnouncementRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Content))
                return BadRequest("Başlık ve içerik zorunludur.");

            var newAnnouncement = new Announcement
            {
                Title = request.Title,
                Content = request.Content,
                Author = string.IsNullOrWhiteSpace(request.Author) ? "Kariyer Merkezi" : request.Author
            };

            await _announcementService.AddAsync(newAnnouncement);
            return Redirect("/announcements");
        }

        [HttpPost("edit/{id}")]
        public async Task<IActionResult> Edit(int id, [FromForm] UpdateAnnouncementRequest request)
        {
            var updated = await _announcementService.UpdateAsync(id, request.Title, request.Content, request.Author, request.IsActive);
            if (!updated) return NotFound();

            return Redirect("/announcements");
        }

        [HttpPost("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _announcementService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return Redirect("/announcements");
        }
    }
}
