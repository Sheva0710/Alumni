using System;
using System.Collections.Generic;
using Alumni.Domain.Entities;

namespace Alumni.Application.DTOs.Users
{
    /// <summary>
    /// Şifre özeti (PasswordHash) içermeyen güvenli kullanıcı yanıt DTO'su.
    /// DATA_DESIGN.md: GET /api/users -> no hash, ever.
    /// </summary>
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "Alumni";
        public DateTime CreatedAt { get; set; }
        public AlumniProfile? Profile { get; set; }
        public List<JobPost>? Jobs { get; set; }

        public static UserResponseDto FromEntity(User user, bool includeJobs = false)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                Profile = user.Profile,
                Jobs = includeJobs ? new List<JobPost>(user.JobPosts ?? new List<JobPost>()) : null
            };
        }
    }
}
