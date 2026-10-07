using Alumni.Application;
using Alumni.Application.DTOs.Users;
using Alumni.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Alumni.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class ApiUserController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            lock (UserStore.UsersLock)
            {
                return Ok(UserStore.Users.ToList());
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            lock (UserStore.UsersLock)
            {
                var user = UserStore.Users.FirstOrDefault(u => u.Id == id);
                if (user == null) return NotFound(new { message = $"ID {id} olan kullanıcı bulunamadı." });
                return Ok(user);
            }
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] CreateUserRequest request)
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
                return Created($"/api/users/{newUser.Id}", newUser);
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            lock (UserStore.UsersLock)
            {
                var user = UserStore.Users.FirstOrDefault(u => u.Id == id);
                if (user == null) return NotFound(new { message = $"ID {id} olan kullanıcı bulunamadı." });

                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.Email = request.Email;
                user.Role = request.Role;

                return Ok(user);
            }
        }

        [HttpPatch("{id}")]
        public IActionResult PatchUser(int id, [FromBody] PatchUserRequest request)
        {
            lock (UserStore.UsersLock)
            {
                var user = UserStore.Users.FirstOrDefault(u => u.Id == id);
                if (user == null) return NotFound(new { message = $"ID {id} olan kullanıcı bulunamadı." });

                if (!string.IsNullOrWhiteSpace(request.FirstName)) user.FirstName = request.FirstName;
                if (!string.IsNullOrWhiteSpace(request.LastName)) user.LastName = request.LastName;
                if (!string.IsNullOrWhiteSpace(request.Email)) user.Email = request.Email;
                if (!string.IsNullOrWhiteSpace(request.Role)) user.Role = request.Role;

                return Ok(user);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            lock (UserStore.UsersLock)
            {
                var user = UserStore.Users.FirstOrDefault(u => u.Id == id);
                if (user == null) return NotFound(new { message = $"ID {id} olan kullanıcı bulunamadı." });

                UserStore.Users.Remove(user);
                return NoContent();
            }
        }
    }
}
