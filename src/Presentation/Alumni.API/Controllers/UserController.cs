using Alumni.Application;
using Alumni.Application.DTOs.Users;
using Alumni.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Alumni.API.Controllers
{
    [Route("user")]
    public class UserController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            lock (UserStore.UsersLock)
            {
                return View(UserStore.Users.ToList());
            }
        }

        [HttpGet("details/{id}")]
        public IActionResult Details(int id)
        {
            lock (UserStore.UsersLock)
            {
                var user = UserStore.Users.FirstOrDefault(u => u.Id == id);
                if (user == null) return NotFound();
                
                return View(user);
            }
        }

        [HttpGet("edit/{id}")]
        public IActionResult Edit(int id)
        {
            lock (UserStore.UsersLock)
            {
                var user = UserStore.Users.FirstOrDefault(u => u.Id == id);
                if (user == null) return NotFound();

                return View(user);
            }
        }

        [HttpPost("create")]
        public IActionResult Create([FromForm] CreateUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.Email))
                return BadRequest("Ad ve e-posta zorunludur.");

            lock (UserStore.UsersLock)
            {
                var newUser = new User
                {
                    Id = UserStore.Users.Count > 0 ? UserStore.Users.Max(u => u.Id) + 1 : 1,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Role = request.Role ?? "User",
                    CreatedAt = DateTime.UtcNow
                };

                UserStore.Users.Add(newUser);
                // Başarılı işlem sonrası MVC'de listeye yönlendirilir
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("edit/{id}")]
        public IActionResult Edit(int id, [FromForm] UpdateUserRequest request)
        {
            lock (UserStore.UsersLock)
            {
                var user = UserStore.Users.FirstOrDefault(u => u.Id == id);
                if (user == null) return NotFound();

                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.Email = request.Email;
                user.Role = request.Role;

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("delete/{id}")]
        public IActionResult Delete(int id)
        {
            lock (UserStore.UsersLock)
            {
                var user = UserStore.Users.FirstOrDefault(u => u.Id == id);
                if (user == null) return NotFound();

                UserStore.Users.Remove(user);
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
