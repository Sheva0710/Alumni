using Alumni.Domain.Entities;
using ApplicationEntity = Alumni.Domain.Entities.Application;

namespace Alumni.API.Common
{
    public class AdminDashboardViewModel
    {
        public List<User> Users { get; set; } = new();
        public List<JobPost> JobPosts { get; set; } = new();
        public List<ApplicationEntity> Applications { get; set; } = new();
        public List<Announcement> Announcements { get; set; } = new();

        public int AdminCount => Users.Count(u => string.Equals(u.Role, "Admin", StringComparison.OrdinalIgnoreCase));
        public int AlumniCount => Users.Count - AdminCount;
        public int ActiveAnnouncementCount => Announcements.Count(a => a.IsActive);
    }
}
