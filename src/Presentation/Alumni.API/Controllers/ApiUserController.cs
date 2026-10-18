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
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound(new { message = $"ID {id} olan kullanıcı bulunamadı." });
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.Email))
                return BadRequest("Ad ve e-posta zorunludur.");

            var newUser = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName ?? string.Empty,
                Email = request.Email,
                Role = request.Role ?? "Alumni"
            };

            await _userService.AddAsync(newUser);
            return Created($"/api/users/{newUser.Id}", newUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            var updated = await _userService.UpdateAsync(id, request.FirstName, request.LastName, request.Email, request.Role);
            if (!updated) return NotFound(new { message = $"ID {id} olan kullanıcı bulunamadı." });

            return Ok(await _userService.GetByIdAsync(id));
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchUser(int id, [FromBody] PatchUserRequest request)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound(new { message = $"ID {id} olan kullanıcı bulunamadı." });

            await _userService.PatchAsync(id, request.FirstName, request.LastName, request.Email, request.Role);
            return Ok(await _userService.GetByIdAsync(id));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleted = await _userService.DeleteAsync(id);
            if (!deleted) return NotFound(new { message = $"ID {id} olan kullanıcı bulunamadı." });

            return NoContent();
        }
    }
}
