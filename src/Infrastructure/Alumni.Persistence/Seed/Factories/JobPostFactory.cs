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
                Title = title.Trim(),
                Company = company.Trim(),
                Description = description.Trim(),
                PostedByUserId = postedByUserId,
                CreatedAt = createdAt ?? DateTime.UtcNow
            };
        }

        /// <summary>
        /// 6. Hafta ders notları (Plate 06 & Şartname) senaryosuna uygun iş ilanları listesi üretir:
        /// - Belirli bir kullanıcıya birden fazla ilan (one-to-many: Backend Intern, Data Analyst, Junior Developer)
        /// - İkinci bir kullanıcıya ilan (QA Tester)
        /// - Diğer kullanıcıların ilanı olmaz (böylece GET /api/users/{id} çağrısında jobs: [] boş dizi testi yapılabilir).
        /// </summary>
        public static List<JobPost> CreateWeek06JobPosts(int primaryUserId, int secondaryUserId)
        {
            return new List<JobPost>
            {
                Create(
                    title: "Backend Intern",
                    company: "Acme",
                    description: "RESTful API geliştirme, PostgreSQL veritabanı ve C# ASP.NET Core bilgisine sahip stajyer.",
                    postedByUserId: primaryUserId
                ),
                Create(
                    title: "Data Analyst",
                    company: "DataCorp",
                    description: "SQL sorguları, raporlama ve veri modelleme konularında mezun takım arkadaşı.",
                    postedByUserId: primaryUserId
                ),
                Create(
                    title: "Junior Developer",
                    company: "Acme",
                    description: "Backend ve API tasarımı alanında tam zamanlı başlangıç pozisyonu.",
                    postedByUserId: primaryUserId
                ),
                Create(
                    title: "QA Tester",
                    company: "TestLab",
                    description: "API testleri (Postman, Swagger), uç nokta ve entegrasyon testlerinde deneyimli adaylar.",
                    postedByUserId: secondaryUserId
                )
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
