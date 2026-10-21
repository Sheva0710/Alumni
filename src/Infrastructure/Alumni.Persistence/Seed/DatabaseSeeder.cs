using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alumni.Domain.Entities;
using Alumni.Persistence.Data;
using Alumni.Persistence.Seed.Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Alumni.Persistence.Seed
{
    /// <summary>
    /// Veritabanını Factory sınıflarını kullanarak ilişkisel bütünlük içinde örnek verilerle dolduran Seeder servisi.
    /// Idempotent çalışır; veritabanında ilgili tabloda kayıt varsa mükerrer ekleme yapmaz.
    /// İstenildiğinde tabloları doğru hiyerarşik sırada temizleyip kimlik sayaçlarını sıfırlayarak (TRUNCATE ... RESTART IDENTITY) yeniden çalışabilir.
    /// </summary>
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(AlumniDbContext context, ILogger? logger = null)
        {
            logger?.LogInformation("--> Veritabanı tohumlama (Seed) kontrolü başlatılıyor...");

            // 1. Kullanıcıların (Users) Tohumlanması
            if (!await context.Users.AnyAsync())
            {
                logger?.LogInformation("--> Kullanıcılar (Users) tohumlanıyor...");
                var adminUser = UserFactory.CreateAdmin();
                var ahmetAlumni = UserFactory.CreateAlumni("Ahmet", "Yılmaz", "ahmet.yilmaz@example.com");
                var ayseAlumni = UserFactory.CreateAlumni("Ayşe", "Demir", "ayse.demir@example.com");
                var zeynepAlumni = UserFactory.CreateAlumni("Zeynep", "Çelik", "zeynep.celik@example.com");
                var employerUser = UserFactory.CreateEmployer("Mehmet", "Kaya", "mehmet.kaya@techcorp.com");

                var users = new List<User> { adminUser, ahmetAlumni, ayseAlumni, zeynepAlumni, employerUser };
                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();
                logger?.LogInformation("--> {Count} adet kullanıcı başarıyla kaydedildi.", users.Count);
            }

            // 2. Mezun Profillerinin (AlumniProfiles) Tohumlanması (1-to-1)
            if (!await context.AlumniProfiles.AnyAsync())
            {
                var ahmet = await context.Users.FirstOrDefaultAsync(u => u.Email == "ahmet.yilmaz@example.com");
                var ayse = await context.Users.FirstOrDefaultAsync(u => u.Email == "ayse.demir@example.com");
                var zeynep = await context.Users.FirstOrDefaultAsync(u => u.Email == "zeynep.celik@example.com");

                if (ahmet != null && ayse != null && zeynep != null)
                {
                    logger?.LogInformation("--> Mezun profilleri (AlumniProfiles) tohumlanıyor...");
                    var profiles = new List<AlumniProfile>
                    {
                        AlumniProfileFactory.Create(
                            userId: ahmet.Id,
                            department: "Bilgisayar Mühendisliği",
                            graduationYear: 2022,
                            currentCompany: "Google",
                            currentTitle: "Software Engineer",
                            linkedInUrl: "https://linkedin.com/in/ahmetyilmaz"
                        ),
                        AlumniProfileFactory.Create(
                            userId: ayse.Id,
                            department: "Endüstri Mühendisliği",
                            graduationYear: 2023,
                            currentCompany: "Amazon",
                            currentTitle: "Data Analyst",
                            linkedInUrl: "https://linkedin.com/in/aysedemir"
                        ),
                        AlumniProfileFactory.Create(
                            userId: zeynep.Id,
                            department: "Yazılım Mühendisliği",
                            graduationYear: 2024,
                            currentCompany: "Microsoft",
                            currentTitle: "DevOps Engineer",
                            linkedInUrl: "https://linkedin.com/in/zeynepcelik"
                        )
                    };

                    await context.AlumniProfiles.AddRangeAsync(profiles);
                    await context.SaveChangesAsync();
                    logger?.LogInformation("--> {Count} adet mezun profili başarıyla oluşturuldu.", profiles.Count);
                }
            }

            // 3. İş İlanlarının (JobPosts) Tohumlanması (1-to-N) — HAFTA 06 GÖREVİ
            await SeedJobPostsAsync(context, logger);

            // 4. İş Başvurularının (Applications) Tohumlanması (Many-to-Many Join Table)
            await SeedApplicationsAsync(context, logger);

            // 5. Duyuruların (Announcements) Tohumlanması
            if (!await context.Announcements.AnyAsync())
            {
                logger?.LogInformation("--> Duyurular (Announcements) tohumlanıyor...");
                var announcements = AnnouncementFactory.CreateDefaultAnnouncements();
                await context.Announcements.AddRangeAsync(announcements);
                await context.SaveChangesAsync();
                logger?.LogInformation("--> {Count} adet duyuru başarıyla oluşturuldu.", announcements.Count);
            }

            logger?.LogInformation("--> Veritabanı tohumlama (Seed) işlemi tamamlandı.");
        }

        /// <summary>
        /// Sadece JobPosts (İş İlanları) tablosunu tohumlayan metot.
        /// Kullanıcıları kontrol eder, uygun kullanıcılara bağlı 1-to-N iş ilanlarını ekler.
        /// </summary>
        public static async Task SeedJobPostsAsync(AlumniDbContext context, ILogger? logger = null)
        {
            if (await context.JobPosts.AnyAsync())
            {
                logger?.LogInformation("--> Veritabanında iş ilanları (JobPosts) zaten mevcut. İlan tohumlama atlandı.");
                return;
            }

            var users = await context.Users.OrderBy(u => u.Id).ToListAsync();
            if (users.Count == 0)
            {
                logger?.LogWarning("--> İş ilanı tohumlamak için veritabanında kullanıcı bulunamadı!");
                return;
            }

            logger?.LogInformation("--> İş İlanları (JobPosts) tohumlanıyor...");

            // İlan sahipleri: Öncelikli olarak Employer ve ilk mezun kullanıcı
            var primaryUser = users.FirstOrDefault(u => u.Role == "Employer") ?? users[0];
            var secondaryUser = users.FirstOrDefault(u => u.Id != primaryUser.Id && u.Role != "Admin") ?? users.Last();

            var jobPosts = JobPostFactory.CreateWeek06JobPosts(primaryUser.Id, secondaryUser.Id);

            await context.JobPosts.AddRangeAsync(jobPosts);
            await context.SaveChangesAsync();
            logger?.LogInformation("--> {Count} adet iş ilanı başarıyla tohumlandı (Kullanıcı {UserId1} ve {UserId2} için).", 
                jobPosts.Count, primaryUser.Id, secondaryUser.Id);
        }

        /// <summary>
        /// Applications ara tablosunu tohumlayan metot.
        /// </summary>
        public static async Task SeedApplicationsAsync(AlumniDbContext context, ILogger? logger = null)
        {
            if (await context.Applications.AnyAsync())
            {
                return;
            }

            var jobs = await context.JobPosts.OrderBy(j => j.Id).ToListAsync();
            var users = await context.Users.Where(u => u.Role != "Admin").OrderBy(u => u.Id).ToListAsync();

            if (jobs.Count >= 2 && users.Count >= 2)
            {
                logger?.LogInformation("--> İş Başvuruları (Applications) tohumlanıyor...");
                var applicant1 = users[0];
                var applicant2 = users[1];

                var applications = ApplicationFactory.CreateWeek06Applications(
                    applicant1Id: applicant1.Id,
                    applicant2Id: applicant2.Id,
                    job1Id: jobs[0].Id,
                    job2Id: jobs[1].Id
                );

                await context.Applications.AddRangeAsync(applications);
                await context.SaveChangesAsync();
                logger?.LogInformation("--> {Count} adet iş başvurusu başarıyla tohumlandı.", applications.Count);
            }
        }

        /// <summary>
        /// 6. Hafta Kuralı: Tabloları doğru sırada (ters bağımlılık) temizleyip sequence sayaçlarını sıfırlayarak
        /// tüm tohumlama işlemini sıfırdan yeniden çalıştırır (idempotency & reset test).
        /// </summary>
        public static async Task ResetAndSeedAsync(AlumniDbContext context, ILogger? logger = null)
        {
            logger?.LogInformation("--> Veritabanı tabloları hiyerarşik sırada temizleniyor (Emptying tables in order)...");

            // Ters bağımlılık sırasıyla silme (Foreign Key ihlallerini engeller)
            try
            {
                // PostgreSQL üzerinde CASCADE ve RESTART IDENTITY ile tam sıfırlama
                await context.Database.ExecuteSqlRawAsync(
                    "TRUNCATE TABLE applications, job_posts, alumni_profiles, users, announcements RESTART IDENTITY CASCADE;"
                );
            }
            catch
            {
                // Alternatif LINQ temizleme fallback
                context.Applications.RemoveRange(context.Applications);
                context.JobPosts.RemoveRange(context.JobPosts);
                context.AlumniProfiles.RemoveRange(context.AlumniProfiles);
                context.Users.RemoveRange(context.Users);
                context.Announcements.RemoveRange(context.Announcements);
                await context.SaveChangesAsync();
            }

            logger?.LogInformation("--> Tablolar temizlendi ve sayaçlar sıfırlandı. Yeniden tohumlama başlatılıyor...");
            await SeedAsync(context, logger);
        }
    }
}
