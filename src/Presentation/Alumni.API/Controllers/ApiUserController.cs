using Alumni.Application.DTOs.Users;
using Alumni.Application.Interfaces;
using Alumni.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Alumni.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Tags("Users")]
    public class ApiUserController : ControllerBase
    {
        private readonly IUserService _userService;

        public ApiUserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            return Ok(_userService.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            var user = _userService.GetById(id);
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
                Role = request.Role ?? "Alumni"
            };

            _userService.Add(newUser);
            return Created($"/api/users/{newUser.Id}", newUser);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            var updated = _userService.Update(id, request.FirstName, request.LastName, request.Email, request.Role);
            if (!updated) return NotFound(new { message = $"ID {id} olan kullanıcı bulunamadı." });

            return Ok(_userService.GetById(id));
        }

        [HttpPatch("{id}")]
        public IActionResult PatchUser(int id, [FromBody] PatchUserRequest request)
        {
            var user = _userService.GetById(id);
            if (user == null) return NotFound(new { message = $"ID {id} olan kullanıcı bulunamadı." });

            _userService.Patch(id, request.FirstName, request.LastName, request.Email, request.Role);
            return Ok(_userService.GetById(id));
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var deleted = _userService.Delete(id);
            if (!deleted) return NotFound(new { message = $"ID {id} olan kullanıcı bulunamadı." });

            return NoContent();
        }
    }
}
