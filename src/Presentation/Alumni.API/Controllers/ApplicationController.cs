using System.Threading.Tasks;
using Alumni.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Alumni.API.Controllers
{
    [Route("applications")]
    [Tags("Applications (MVC)")]
    public class ApplicationController : Controller
    {
        private readonly IApplicationService _applicationService;
        private readonly IJobPostService _jobPostService;
        private readonly IUserService _userService;

        public ApplicationController(
            IApplicationService applicationService,
            IJobPostService jobPostService,
            IUserService userService)
        {
            _applicationService = applicationService;
            _jobPostService = jobPostService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var applications = await _applicationService.GetAllAsync();
            ViewBag.Users = await _userService.GetAllAsync();
            ViewBag.Jobs = await _jobPostService.GetAllAsync();

            return View(applications);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] int userId, [FromForm] int jobPostId)
        {
            if (userId <= 0 || jobPostId <= 0)
            {
                TempData["ErrorMessage"] = "Lütfen geçerli bir aday ve iş ilanı seçiniz.";
                return Redirect("/applications");
            }

            var result = await _applicationService.ApplyAsync(jobPostId, userId);
            if (result.Status == ApplyResultStatus.Success)
            {
                TempData["SuccessMessage"] = "İş başvurusu başarıyla oluşturuldu.";
            }
            else if (result.Status == ApplyResultStatus.AlreadyApplied)
            {
                TempData["ErrorMessage"] = "Seçilen aday bu ilana daha önce başvuruda bulunmuş (Mükerrer başvuru engellendi).";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? "Başvuru oluşturulamadı.";
            }

            return Redirect("/applications");
        }

        [HttpPost("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromForm] string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                TempData["ErrorMessage"] = "Durum alanı boş olamaz.";
                return Redirect("/applications");
            }

            var updated = await _applicationService.UpdateStatusAsync(id, status.Trim());
            if (updated)
            {
                TempData["SuccessMessage"] = $"Başvuru durumu '{status}' olarak güncellendi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Başvuru bulunamadı.";
            }

            return Redirect("/applications");
        }

        [HttpPost("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _applicationService.DeleteAsync(id);
            if (deleted)
            {
                TempData["SuccessMessage"] = "Başvuru başarıyla iptal edildi / silindi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Başvuru bulunamadı.";
            }

            return Redirect("/applications");
        }
    }
}
