using System.Reflection;
using Alumni.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Alumni.Persistence.Data
{
    public class AlumniDbContext : DbContext
    {
        public AlumniDbContext(DbContextOptions<AlumniDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<AlumniProfile> AlumniProfiles => Set<AlumniProfile>();
        public DbSet<JobPost> JobPosts => Set<JobPost>();
        public DbSet<Announcement> Announcements => Set<Announcement>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
