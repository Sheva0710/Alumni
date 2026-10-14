using Alumni.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alumni.Persistence.Configurations
{
    public class AlumniProfileConfiguration : IEntityTypeConfiguration<AlumniProfile>
    {
        public void Configure(EntityTypeBuilder<AlumniProfile> builder)
        {
            builder.ToTable("alumni_profiles", t =>
                t.HasCheckConstraint("chk_alumni_profiles_grad_year", "graduation_year >= 1950 AND graduation_year <= 2100"));

            builder.HasKey(p => p.Id).HasName("pk_alumni_profiles");
            builder.Property(p => p.Id).HasColumnName("id").ValueGeneratedOnAdd();

            builder.Property(p => p.UserId).HasColumnName("user_id").IsRequired();
            builder.HasIndex(p => p.UserId).IsUnique().HasDatabaseName("ix_alumni_profiles_user_id");

            builder.Property(p => p.Department).HasColumnName("department").HasMaxLength(150).IsRequired();
            builder.Property(p => p.GraduationYear).HasColumnName("graduation_year").IsRequired();
            builder.Property(p => p.CurrentCompany).HasColumnName("current_company").HasMaxLength(200).IsRequired(false);
            builder.Property(p => p.CurrentTitle).HasColumnName("current_title").HasMaxLength(150).IsRequired(false);
            builder.Property(p => p.LinkedInUrl).HasColumnName("linkedin_url").HasMaxLength(500).IsRequired(false);
        }
    }
}
