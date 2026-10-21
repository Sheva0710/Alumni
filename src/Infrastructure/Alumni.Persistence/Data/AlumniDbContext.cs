using System.Reflection;
using Alumni.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Application = Alumni.Domain.Entities.Application;

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
        public DbSet<Alumni.Domain.Entities.Application> Applications => Set<Alumni.Domain.Entities.Application>();
        public DbSet<Announcement> Announcements => Set<Announcement>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
