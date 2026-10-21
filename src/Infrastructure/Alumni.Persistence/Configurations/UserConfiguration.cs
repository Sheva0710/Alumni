using Alumni.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alumni.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(u => u.Id).HasName("pk_users");
            builder.Property(u => u.Id).HasColumnName("id").ValueGeneratedOnAdd();

            builder.Property(u => u.FirstName).HasColumnName("first_name").HasMaxLength(100).IsRequired();
            builder.Property(u => u.LastName).HasColumnName("last_name").HasMaxLength(100).IsRequired();
            builder.Property(u => u.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
            builder.HasIndex(u => u.Email).IsUnique().HasDatabaseName("ix_users_email");

            builder.Property(u => u.Role).HasColumnName("role").HasMaxLength(50).IsRequired().HasDefaultValue("Alumni");
            builder.Property(u => u.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

            // 1-to-1 Relationship with AlumniProfile (Cascade Delete)
            builder.HasOne(u => u.Profile)
                   .WithOne(p => p.User)
                   .HasForeignKey<AlumniProfile>(p => p.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // 1-to-N Relationship with JobPosts (Restrict Delete - DATA_DESIGN.md)
            builder.HasMany(u => u.JobPosts)
                   .WithOne(j => j.PostedByUser)
                   .HasForeignKey(j => j.PostedByUserId)
                   .OnDelete(DeleteBehavior.Restrict);

            // 1-to-N Relationship with Applications
            builder.HasMany(u => u.Applications)
                   .WithOne(a => a.User)
                   .HasForeignKey(a => a.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
