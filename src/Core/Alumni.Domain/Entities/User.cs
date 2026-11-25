using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Alumni.Domain.Entities
{
    /// <summary>
    /// Sistem kullanıcısı (Mezun / Alumni, Yönetici / Admin).
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// BCrypt (hash + salt) şifre özeti. API yanıtlarında asla dışarıya döndürülmez.
        /// </summary>
        [JsonIgnore]
        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "Alumni";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // 1-to-1 Relationship with AlumniProfile (Cascade Delete)
        public AlumniProfile? Profile { get; set; }

        // 1-to-N Relationship with JobPost (Migration: Restrict Delete - DATA_DESIGN.md)
        public ICollection<JobPost> JobPosts { get; set; } = new List<JobPost>();

        // 1-to-N Relationship with Application (Migration: Cascade Delete)
        public ICollection<Application> Applications { get; set; } = new List<Application>();
    }
}
