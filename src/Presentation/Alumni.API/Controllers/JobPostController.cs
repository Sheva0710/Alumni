using System.Threading.Tasks;
using Alumni.API.Common;
using Alumni.Application.DTOs.Jobs;
using Alumni.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Alumni.API.Controllers
{
    [Route("jobs")]
    [Route("job-posts")]
    [Tags("Job Postings (MVC)")]
    public class JobPostController : Controller
    {
        private readonly IJobPostService _jobPostService;
        private readonly IApplicationService _applicationService;
        private readonly IUserService _userService;

        public JobPostController(
            IJobPostService jobPostService,
            IApplicationService applicationService,
            IUserService userService)
        {
            _jobPostService = jobPostService;
            _applicationService = applicationService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var jobs = await _jobPostService.GetAllAsync();
            ViewBag.Users = await _userService.GetAllAsync();
            return View(jobs);
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var job = await _jobPostService.GetByIdAsync(id);
            if (job == null) return NotFound();

            var applicants = await _applicationService.GetApplicantsByJobIdAsync(id);
            var users = await _userService.GetAllAsync();

            ViewBag.Applicants = applicants;
            ViewBag.Users = users;

            return View(job);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(
            [FromForm] int postedByUserId,
            [FromForm] string title,
            [FromForm] string company,
            [FromForm] string? city,
            [FromForm] string? description)
        {
            // Güvenlik (own): Alumni yalnızca kendi adına ilan açabilir; Admin kullanıcı seçebilir.
            var effectiveOwnerId = User.IsAdminRole()
                ? postedByUserId
                : (User.GetUserId() ?? 0);

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(company) || effectiveOwnerId <= 0)
            {
                TempData["ErrorMessage"] = "Başlık, şirket ve ilan sahibi kullanıcı bilgisi zorunludur.";
                return Redirect("/jobs");
            }

            if (title.Trim().Length > 120)
            {
                TempData["ErrorMessage"] = "İlan başlığı en fazla 120 karakter olabilir.";
                return Redirect("/jobs");
            }

            var created = await _jobPostService.CreateForUserAsync(effectiveOwnerId, title, company, city, description);
            if (created == null)
            {
                TempData["ErrorMessage"] = "Seçilen kullanıcı bulunamadı.";
            }
            else
            {
                TempData["SuccessMessage"] = $"'{title}' ilanı başarıyla oluşturuldu.";
            }

            return Redirect("/jobs");
        }

        [HttpPost("{id}/apply")]
        public async Task<IActionResult> Apply(int id, [FromForm] int userId)
        {
            // Güvenlik (self): Formdaki userId yok sayılır; başvuran daima oturumdaki kullanıcıdır.
            var currentUserId = User.GetUserId();
            if (!currentUserId.HasValue)
            {
                return Redirect($"/login?returnUrl=/jobs/details/{id}");
            }

            if (User.IsAdminRole())
            {
                TempData["ErrorMessage"] = "Admin rolündeki kullanıcılar iş ilanlarına başvuru yapamaz.";
                return Redirect($"/jobs/details/{id}");
            }

            var job = await _jobPostService.GetByIdAsync(id);
            if (job != null && job.PostedByUserId == currentUserId.Value)
            {
                TempData["ErrorMessage"] = "Kendi yayınladığınız iş ilanına başvuru yapamazsınız.";
                return Redirect($"/jobs/details/{id}");
            }

            var result = await _applicationService.ApplyAsync(id, currentUserId.Value);
            if (result.Status == ApplyResultStatus.Success)
            {
                TempData["SuccessMessage"] = "Başvurunuz başarıyla kaydedildi.";
            }
            else if (result.Status == ApplyResultStatus.AlreadyApplied)
            {
                TempData["ErrorMessage"] = "Bu ilana daha önce başvuru yapılmış (Mükerrer başvuru engellendi).";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? "Başvuru yapılamadı.";
            }

            return Redirect($"/jobs/details/{id}");
        }

        [HttpPost("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var job = await _jobPostService.GetByIdAsync(id);
            if (job == null)
            {
                TempData["ErrorMessage"] = "İş ilanı bulunamadı.";
                return Redirect("/jobs");
            }

            if (!User.IsAdminRole() && job.PostedByUserId != User.GetUserId())
            {
                TempData["ErrorMessage"] = "Yalnızca kendi yayınladığınız ilanları silebilirsiniz.";
                return Redirect("/jobs");
            }

            await _jobPostService.DeleteAsync(id);
            TempData["SuccessMessage"] = "İş ilanı ve bağlı tüm başvuruları başarıyla silindi (Cascade).";
            return Redirect("/jobs");
        }
    }
}
