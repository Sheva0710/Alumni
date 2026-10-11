using Alumni.Application;
using Alumni.Application.DTOs.Users;
using Alumni.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Alumni.API.Controllers
{
    [Route("users")]
    [Route("user")]
    [Tags("Users (MVC)")]
    public class UserController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(UserStore.GetAll());
        }

        [HttpGet("details/{id}")]
        public IActionResult Details(int id)
        {
            var user = UserStore.GetById(id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpGet("edit/{id}")]
        public IActionResult Edit(int id)
        {
            var user = UserStore.GetById(id);
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
                Role = request.Role ?? "User"
            };

            UserStore.Add(newUser);
            return Redirect("/users");
        }

        [HttpPost("edit/{id}")]
        public IActionResult Edit(int id, [FromForm] UpdateUserRequest request)
        {
            var updated = UserStore.Update(id, request.FirstName, request.LastName, request.Email, request.Role);
            if (!updated) return NotFound();

            return Redirect("/users");
        }

        [HttpPost("delete/{id}")]
        public IActionResult Delete(int id)
        {
            var deleted = UserStore.Delete(id);
            if (!deleted) return NotFound();

            return Redirect("/users");
        }
    }
}

