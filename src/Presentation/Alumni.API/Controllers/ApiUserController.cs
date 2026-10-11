using Alumni.Application;
using Alumni.Application.DTOs.Users;
using Alumni.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Alumni.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Tags("Users")]
    public class ApiUserController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            return Ok(UserStore.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            var user = UserStore.GetById(id);
            if (user == null) return NotFound(new { message = $"ID {id} olan kullanıcı bulunamadı." });
            return Ok(user);
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] CreateUserRequest request)
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
            return Created($"/api/users/{newUser.Id}", newUser);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            var updated = UserStore.Update(id, request.FirstName, request.LastName, request.Email, request.Role);
            if (!updated) return NotFound(new { message = $"ID {id} olan kullanıcı bulunamadı." });

            return Ok(UserStore.GetById(id));
        }

        [HttpPatch("{id}")]
        public IActionResult PatchUser(int id, [FromBody] PatchUserRequest request)
        {
            var user = UserStore.GetById(id);
            if (user == null) return NotFound(new { message = $"ID {id} olan kullanıcı bulunamadı." });

            UserStore.Patch(id, request.FirstName, request.LastName, request.Email, request.Role);
            return Ok(UserStore.GetById(id));
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var deleted = UserStore.Delete(id);
            if (!deleted) return NotFound(new { message = $"ID {id} olan kullanıcı bulunamadı." });

            return NoContent();
        }
    }
}

