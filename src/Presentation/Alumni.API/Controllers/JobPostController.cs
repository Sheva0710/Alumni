using System.Threading.Tasks;
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
        public async Task<IActionResult> Create([FromForm] int postedByUserId, [FromForm] string title, [FromForm] string company, [FromForm] string description)
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(company) || postedByUserId <= 0)
            {
                TempData["ErrorMessage"] = "Başlık, şirket ve ilan sahibi kullanıcı seçimi zorunludur.";
                return Redirect("/jobs");
            }

            var created = await _jobPostService.CreateForUserAsync(postedByUserId, title, company, description ?? string.Empty);
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
            if (userId <= 0)
            {
                TempData["ErrorMessage"] = "Lütfen başvuran bir kullanıcı seçiniz.";
                return Redirect($"/jobs/details/{id}");
            }

            var result = await _applicationService.ApplyAsync(id, userId);
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
            var deleted = await _jobPostService.DeleteAsync(id);
            if (deleted)
            {
                TempData["SuccessMessage"] = "İş ilanı ve bağlı tüm başvuruları başarıyla silindi (Cascade).";
            }
            else
            {
                TempData["ErrorMessage"] = "İş ilanı bulunamadı.";
            }

            return Redirect("/jobs");
        }
    }
}
