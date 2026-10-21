using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApplicationEntity = Alumni.Domain.Entities.Application;

namespace Alumni.Persistence.Configurations
{
    public class ApplicationConfiguration : IEntityTypeConfiguration<ApplicationEntity>
    {
        public void Configure(EntityTypeBuilder<ApplicationEntity> builder)
        {
            builder.ToTable("applications");

            builder.HasKey(a => a.Id).HasName("pk_applications");
            builder.Property(a => a.Id).HasColumnName("id").ValueGeneratedOnAdd();

            builder.Property(a => a.UserId).HasColumnName("user_id").IsRequired();
            builder.Property(a => a.JobPostId).HasColumnName("job_post_id").IsRequired();

            builder.Property(a => a.Status).HasColumnName("status").HasMaxLength(50).IsRequired().HasDefaultValue("Applied");
            builder.Property(a => a.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Unique Pair Constraint: UNIQUE (user_id, job_post_id)
            builder.HasIndex(a => new { a.UserId, a.JobPostId })
                   .IsUnique()
                   .HasDatabaseName("ix_applications_user_id_job_post_id");

            builder.HasIndex(a => a.UserId).HasDatabaseName("ix_applications_user_id");
            builder.HasIndex(a => a.JobPostId).HasDatabaseName("ix_applications_job_post_id");

            // Relationships
            builder.HasOne(a => a.User)
                   .WithMany(u => u.Applications)
                   .HasForeignKey(a => a.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.JobPost)
                   .WithMany(j => j.Applications)
                   .HasForeignKey(a => a.JobPostId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
