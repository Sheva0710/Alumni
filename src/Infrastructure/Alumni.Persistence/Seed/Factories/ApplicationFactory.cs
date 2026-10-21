using System;
using System.Collections.Generic;
using ApplicationEntity = Alumni.Domain.Entities.Application;

namespace Alumni.Persistence.Seed.Factories
{
    /// <summary>
    /// Applications ara tablosu için örnek başvuru verileri üreten fabrika sınıfı.
    /// Tekil çift UNIQUE(user_id, job_post_id) kuralına uygundur.
    /// </summary>
    public static class ApplicationFactory
    {
        public static ApplicationEntity Create(
            int userId,
            int jobPostId,
            string status = "Applied",
            DateTime? createdAt = null)
        {
            return new ApplicationEntity
            {
                UserId = userId,
                JobPostId = jobPostId,
                Status = status,
                CreatedAt = createdAt ?? DateTime.UtcNow
            };
        }

        /// <summary>
        /// 6. Hafta ders notları (Plate 06) senaryosuna uygun örnek başvurular:
        /// - İlan 1'e iki farklı aday başvurur (örn. 3 ve 2 numaralı kullanıcılar).
        /// - İlan 2'ye bir aday "Interview" durumunda başvurur.
        /// - Mükerrer çift içermez.
        /// </summary>
        public static List<ApplicationEntity> CreateWeek06Applications(int applicant1Id, int applicant2Id, int job1Id, int job2Id)
        {
            return new List<ApplicationEntity>
            {
                Create(userId: applicant1Id, jobPostId: job1Id, status: "Applied"),
                Create(userId: applicant2Id, jobPostId: job1Id, status: "Applied"),
                Create(userId: applicant1Id, jobPostId: job2Id, status: "Interview")
            };
        }
    }
}
