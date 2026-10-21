using System;
using System.Text.Json.Serialization;

namespace Alumni.Domain.Entities
{
    /// <summary>
    /// Kullanıcılar ile iş ilanları arasındaki başvuru ilişkisini temsil eden Many-to-Many ara tablo varlığı (Join Entity).
    /// Migration: 'applications' tablosu, tekil çift kuralı UNIQUE(user_id, job_post_id).
    /// </summary>
    public class Application
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int JobPostId { get; set; }
        public string Status { get; set; } = "Applied";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties (Migration: FK_applications_users_user_id & FK_applications_job_posts_job_post_id)
        [JsonIgnore]
        public User? User { get; set; }

        [JsonIgnore]
        public JobPost? JobPost { get; set; }
    }
}
