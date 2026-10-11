using Alumni.Application.DTOs.Announcements;
using Alumni.Application.Interfaces;
using Alumni.Domain.Entities;
using Microsoft.AspNetCore.Http;
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
        public IActionResult Index()
        {
            var announcements = _announcementService.GetAll();
            return View(announcements);
        }

        [HttpGet("details/{id}")]
        public IActionResult Details(int id)
        {
            var announcement = _announcementService.GetById(id);
            if (announcement == null) return NotFound();

            return View(announcement);
        }

        [HttpGet("edit/{id}")]
        public IActionResult Edit(int id)
        {
            var announcement = _announcementService.GetById(id);
            if (announcement == null) return NotFound();

            return View(announcement);
        }

        [HttpPost]
        [HttpPost("create")]
        public IActionResult Create([FromForm] CreateAnnouncementRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Content))
                return BadRequest("Başlık ve içerik zorunludur.");

            var newAnnouncement = new Announcement
            {
                Title = request.Title,
                Content = request.Content,
                Author = string.IsNullOrWhiteSpace(request.Author) ? "Kariyer Merkezi" : request.Author
            };

            _announcementService.Add(newAnnouncement);
            return Redirect("/announcements");
        }

        [HttpPost("edit/{id}")]
        public IActionResult Edit(int id, [FromForm] UpdateAnnouncementRequest request)
        {
            var updated = _announcementService.Update(id, request.Title, request.Content, request.Author, request.IsActive);
            if (!updated) return NotFound();

            return Redirect("/announcements");
        }

        [HttpPost("delete/{id}")]
        public IActionResult Delete(int id)
        {
            var deleted = _announcementService.Delete(id);
            if (!deleted) return NotFound();

            return Redirect("/announcements");
        }
    }
}
