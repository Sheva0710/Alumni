using System;
using System.Collections.Generic;
using Alumni.Domain.Entities;

namespace Alumni.Persistence.Seed.Factories
{
    /// <summary>
    /// User varlıkları için örnek ve test verileri üreten fabrika sınıfı.
    /// DATA_DESIGN.md kurallarına uygun olarak PascalCase ve geçerli alan değerleri sağlar.
    /// </summary>
    public static class UserFactory
    {
        public static User Create(
            string firstName,
            string lastName,
            string email,
            string role = "Alumni",
            DateTime? createdAt = null)
        {
            return new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email.ToLowerInvariant().Trim(),
                Role = role,
                CreatedAt = createdAt ?? DateTime.UtcNow
            };
        }

        public static User CreateAdmin(string firstName = "Sistem", string lastName = "Yöneticisi", string email = "admin@alumni.edu.tr")
        {
            return Create(firstName, lastName, email, role: "Admin");
        }

        public static User CreateAlumni(string firstName, string lastName, string email)
        {
            return Create(firstName, lastName, email, role: "Alumni");
        }

        public static User CreateEmployer(string firstName, string lastName, string email)
        {
            return Create(firstName, lastName, email, role: "Employer");
        }

        /// <summary>
        /// İlk çalıştırma (Seed) için gerçekçi başlangıç kullanıcı listesi üretir.
        /// </summary>
        public static List<User> CreateDefaultUsers()
        {
            return new List<User>
            {
                Create("Sistem", "Yöneticisi", "admin@alumni.edu.tr", "Admin"),
                Create("Ahmet", "Yılmaz", "ahmet.yilmaz@example.com", "Alumni"),
                Create("Ayşe", "Demir", "ayse.demir@example.com", "Alumni"),
                Create("Mehmet", "Kaya", "mehmet.kaya@techcorp.com", "Employer"),
                Create("Zeynep", "Çelik", "zeynep.celik@example.com", "Alumni")
            };
        }
    }
}
