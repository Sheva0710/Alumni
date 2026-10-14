using System;
using Alumni.Domain.Entities;

namespace Alumni.Persistence.Seed.Factories
{
    /// <summary>
    /// AlumniProfile varlıkları için örnek veriler üreten fabrika sınıfı.
    /// DATA_DESIGN.md kısıtlarına (chk_alumni_profiles_grad_year vb.) tam uyumlu veri üretir.
    /// </summary>
    public static class AlumniProfileFactory
    {
        public static AlumniProfile Create(
            int userId,
            string department,
            int graduationYear,
            string? currentCompany = null,
            string? currentTitle = null,
            string? linkedInUrl = null)
        {
            if (graduationYear < 1950 || graduationYear > 2100)
            {
                throw new ArgumentOutOfRangeException(nameof(graduationYear), "Mezuniyet yılı 1950 ile 2100 arasında olmalıdır (DATA_DESIGN.md kısıtı).");
            }

            return new AlumniProfile
            {
                UserId = userId,
                Department = department,
                GraduationYear = graduationYear,
                CurrentCompany = currentCompany,
                CurrentTitle = currentTitle,
                LinkedInUrl = linkedInUrl
            };
        }

        public static AlumniProfile CreateForUser(
            User user,
            string department,
            int graduationYear,
            string? currentCompany = null,
            string? currentTitle = null,
            string? linkedInUrl = null)
        {
            var profile = Create(user.Id, department, graduationYear, currentCompany, currentTitle, linkedInUrl);
            profile.User = user;
            return profile;
        }
    }
}
