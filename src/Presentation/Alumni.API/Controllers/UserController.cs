using Alumni.API.Common;
using Alumni.Application.DTOs.Users;
using Alumni.Application.Interfaces;
using Alumni.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Alumni.API.Controllers
{
    [Route("users")]
    [Route("user")]
    [Tags("Users (MVC)")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllAsync();
            return View(users);
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CreateUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.Email))
                return BadRequest("Ad ve e-posta zorunludur.");

            var assignedRole = User.IsAdminRole() && !string.IsNullOrWhiteSpace(request.Role)
                ? request.Role
                : "Alumni";

            var newUser = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName ?? string.Empty,
                Email = request.Email,
                Role = assignedRole
            };

            await _userService.AddAsync(newUser);
            return Redirect("/users");
        }

        [HttpPost("edit/{id}")]
        public async Task<IActionResult> Edit(int id, [FromForm] UpdateUserRequest request)
        {
            // Güvenlik: Alumni kendi profilini düzenlerken rol yükseltemez
            var safeRole = User.IsAdminRole() ? request.Role : null;
            var updated = await _userService.UpdateAsync(id, request.FirstName, request.LastName, request.Email, safeRole);
            if (!updated) return NotFound();

            return Redirect("/users");
        }

        [HttpPost("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _userService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return Redirect("/users");
        }
    }
}
