using System.Linq;
using System.Threading.Tasks;
using Alumni.API.Common;
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
            if (!User.IsAdminRole())
            {
                var currentUserId = User.GetUserId();
                applications = applications.Where(a => a.UserId == currentUserId).ToList();
            }

            ViewBag.Users = await _userService.GetAllAsync();
            ViewBag.Jobs = await _jobPostService.GetAllAsync();

            return View(applications);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] int userId, [FromForm] int jobPostId)
        {
            // Güvenlik (self): Başvuran daima oturumdaki kullanıcıdır; formdaki userId yok sayılır.
            var currentUserId = User.GetUserId();
            if (!currentUserId.HasValue)
            {
                return Redirect("/login?returnUrl=/applications");
            }

            if (User.IsAdminRole())
            {
                TempData["ErrorMessage"] = "Admin rolündeki kullanıcılar iş ilanlarına başvuru yapamaz.";
                return Redirect("/applications");
            }

            if (jobPostId <= 0)
            {
                TempData["ErrorMessage"] = "Lütfen geçerli bir iş ilanı seçiniz.";
                return Redirect("/applications");
            }

            var job = await _jobPostService.GetByIdAsync(jobPostId);
            if (job != null && job.PostedByUserId == currentUserId.Value)
            {
                TempData["ErrorMessage"] = "Kendi yayınladığınız iş ilanına başvuru yapamazsınız.";
                return Redirect("/applications");
            }

            var result = await _applicationService.ApplyAsync(jobPostId, currentUserId.Value);
            if (result.Status == ApplyResultStatus.Success)
            {
                TempData["SuccessMessage"] = "İş başvurusu başarıyla oluşturuldu.";
            }
            else if (result.Status == ApplyResultStatus.AlreadyApplied)
            {
                TempData["ErrorMessage"] = "Bu ilana daha önce başvuruda bulundunuz (Mükerrer başvuru engellendi).";
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
            var application = await _applicationService.GetByIdAsync(id);
            if (application == null)
            {
                TempData["ErrorMessage"] = "Başvuru bulunamadı.";
                return Redirect("/applications");
            }

            if (!User.IsAdminRole() && application.UserId != User.GetUserId())
            {
                TempData["ErrorMessage"] = "Yalnızca kendi başvurunuzu iptal edebilirsiniz.";
                return Redirect("/applications");
            }

            await _applicationService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Başvuru başarıyla iptal edildi / silindi.";
            return Redirect("/applications");
        }
    }
}
