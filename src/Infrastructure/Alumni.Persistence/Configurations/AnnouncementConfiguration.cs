using Alumni.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alumni.Persistence.Configurations
{
    public class AnnouncementConfiguration : IEntityTypeConfiguration<Announcement>
    {
        public void Configure(EntityTypeBuilder<Announcement> builder)
        {
            builder.ToTable("announcements");

            builder.HasKey(a => a.Id).HasName("pk_announcements");
            builder.Property(a => a.Id).HasColumnName("id").ValueGeneratedOnAdd();

            builder.Property(a => a.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
            builder.Property(a => a.Content).HasColumnName("content").HasColumnType("text").IsRequired();
            builder.Property(a => a.Author).HasColumnName("author").HasMaxLength(100).IsRequired().HasDefaultValue("Kariyer Merkezi");
            builder.Property(a => a.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(a => a.IsActive).HasColumnName("is_active").IsRequired().HasDefaultValue(true);
        }
    }
}
