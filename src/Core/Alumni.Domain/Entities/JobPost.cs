using System.Text.Json.Serialization;

namespace Alumni.Domain.Entities
{
    /// <summary>
    /// Bir kullanıcı tarafından açılan iş veya staj ilanı (1-to-N).
    /// </summary>
    public class JobPost
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int PostedByUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties (Migration: FK_job_posts_users_posted_by_user_id)
        [JsonIgnore]
        public User? PostedByUser { get; set; }

        // Migration: FK_applications_job_posts_job_post_id (Cascade)
        public ICollection<Application> Applications { get; set; } = new List<Application>();
    }
}
