using System;
using System.Collections.Generic;
using Alumni.Domain.Entities;

namespace Alumni.Persistence.Seed.Factories
{
    /// <summary>
    /// JobPost varlıkları için örnek iş/staj ilanları üreten fabrika sınıfı.
    /// DATA_DESIGN.md kurallarına ve ilişkisel foreign key standartlarına tam uygundur.
    /// </summary>
    public static class JobPostFactory
    {
        public static JobPost Create(
            string title,
            string company,
            string description,
            int postedByUserId,
            DateTime? createdAt = null)
        {
            return new JobPost
            {
                Title = title,
                Company = company,
                Description = description,
                PostedByUserId = postedByUserId,
                CreatedAt = createdAt ?? DateTime.UtcNow
            };
        }

        public static List<JobPost> CreateDefaultJobPosts(int postedByUserId)
        {
            return new List<JobPost>
            {
                Create(
                    title: "Senior .NET & Cloud Backend Developer",
                    company: "TechCorp Solutions",
                    description: "Mikroservis mimarisinde, PostgreSQL ve ASP.NET Core deneyimi olan mezun çalışma arkadaşları arıyoruz.",
                    postedByUserId: postedByUserId
                ),
                Create(
                    title: "Junior Full Stack Developer",
                    company: "Inovasyon Yazılım A.Ş.",
                    description: "C#, EF Core ve modern ön yüz teknolojilerine ilgi duyan yeni mezunlar için tam zamanlı pozisyon.",
                    postedByUserId: postedByUserId
                ),
                Create(
                    title: "Veri Tabanı Yöneticisi & Veri Mühendisi (PostgreSQL)",
                    company: "DataDynamics",
                    description: "İlişkisel veritabanı tasarımı, indeksleme, optimizasyon ve ETL süreçlerinde görev alacak mezunlar.",
                    postedByUserId: postedByUserId
                )
            };
        }
    }
}
