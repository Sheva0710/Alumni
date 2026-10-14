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
        public IActionResult Index()
        {
            return View(_userService.GetAll());
        }

        [HttpGet("details/{id}")]
        public IActionResult Details(int id)
        {
            var user = _userService.GetById(id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpGet("edit/{id}")]
        public IActionResult Edit(int id)
        {
            var user = _userService.GetById(id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        [HttpPost("create")]
        public IActionResult Create([FromForm] CreateUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.Email))
                return BadRequest("Ad ve e-posta zorunludur.");

            var newUser = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Role = request.Role ?? "Alumni"
            };

            _userService.Add(newUser);
            return Redirect("/users");
        }

        [HttpPost("edit/{id}")]
        public IActionResult Edit(int id, [FromForm] UpdateUserRequest request)
        {
            var updated = _userService.Update(id, request.FirstName, request.LastName, request.Email, request.Role);
            if (!updated) return NotFound();

            return Redirect("/users");
        }

        [HttpPost("delete/{id}")]
        public IActionResult Delete(int id)
        {
            var deleted = _userService.Delete(id);
            if (!deleted) return NotFound();

            return Redirect("/users");
        }
    }
}
