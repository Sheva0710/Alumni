using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alumni.Domain.Entities;
using Alumni.Persistence.Data;
using Alumni.Persistence.Seed.Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Alumni.Persistence.Seed
{
    /// <summary>
    /// Veritabanını ilk ayağa kalkışta Factory sınıflarını kullanarak örnek verilerle dolduran Seeder servisi.
    /// Idempotent çalışır; veritabanında kayıt varsa tekrar ekleme yapmaz.
    /// </summary>
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(AlumniDbContext context, ILogger? logger = null)
        {
            // 1. Kullanıcı kontrolü (Veritabanı zaten dolu mu?)
            if (await context.Users.AnyAsync())
            {
                logger?.LogInformation("--> Veritabanında mevcut kullanıcılar bulundu. Seed işlemi atlanıyor.");
                return;
            }

            logger?.LogInformation("--> Veritabanı boş, Factory sınıfları kullanılarak tohumlama (Seed) başlatılıyor...");

            // 2. Kullanıcıların (Users) Factory ile Üretilmesi
            var adminUser = UserFactory.CreateAdmin();
            var ahmetAlumni = UserFactory.CreateAlumni("Ahmet", "Yılmaz", "ahmet.yilmaz@example.com");
            var ayseAlumni = UserFactory.CreateAlumni("Ayşe", "Demir", "ayse.demir@example.com");
            var zeynepAlumni = UserFactory.CreateAlumni("Zeynep", "Çelik", "zeynep.celik@example.com");
            var employerUser = UserFactory.CreateEmployer("Mehmet", "Kaya", "mehmet.kaya@techcorp.com");

            var users = new List<User> { adminUser, ahmetAlumni, ayseAlumni, zeynepAlumni, employerUser };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
            logger?.LogInformation("--> {Count} adet kullanıcı başarıyla kaydedildi.", users.Count);

            // 3. Mezun Profillerinin (AlumniProfiles) Factory ile Üretilmesi (1-to-1)
            var profiles = new List<AlumniProfile>
            {
                AlumniProfileFactory.Create(
                    userId: ahmetAlumni.Id,
                    department: "Bilgisayar Mühendisliği",
                    graduationYear: 2022,
                    currentCompany: "Google",
                    currentTitle: "Software Engineer",
                    linkedInUrl: "https://linkedin.com/in/ahmetyilmaz"
                ),
                AlumniProfileFactory.Create(
                    userId: ayseAlumni.Id,
                    department: "Endüstri Mühendisliği",
                    graduationYear: 2023,
                    currentCompany: "Amazon",
                    currentTitle: "Data Analyst",
                    linkedInUrl: "https://linkedin.com/in/aysedemir"
                ),
                AlumniProfileFactory.Create(
                    userId: zeynepAlumni.Id,
                    department: "Yazılım Mühendisliği",
                    graduationYear: 2024,
                    currentCompany: "Microsoft",
                    currentTitle: "DevOps Engineer",
                    linkedInUrl: "https://linkedin.com/in/zeynepcelik"
                )
            };

            await context.AlumniProfiles.AddRangeAsync(profiles);
            logger?.LogInformation("--> {Count} adet mezun profili başarıyla oluşturuldu.", profiles.Count);

            // 4. İş İlanlarının (JobPosts) Factory ile Üretilmesi (1-to-N)
            var jobPosts = JobPostFactory.CreateDefaultJobPosts(employerUser.Id);
            await context.JobPosts.AddRangeAsync(jobPosts);
            logger?.LogInformation("--> {Count} adet iş ilanı başarıyla oluşturuldu.", jobPosts.Count);

            // 5. Duyuruların (Announcements) Factory ile Üretilmesi
            if (!await context.Announcements.AnyAsync())
            {
                var announcements = AnnouncementFactory.CreateDefaultAnnouncements();
                await context.Announcements.AddRangeAsync(announcements);
                logger?.LogInformation("--> {Count} adet duyuru başarıyla oluşturuldu.", announcements.Count);
            }

            await context.SaveChangesAsync();
            logger?.LogInformation("--> Veritabanı tohumlama (Seed) işlemi başarıyla tamamlandı!");
        }
    }
}
