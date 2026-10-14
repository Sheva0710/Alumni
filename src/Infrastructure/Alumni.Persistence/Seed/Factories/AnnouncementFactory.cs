using System;
using System.Collections.Generic;
using Alumni.Domain.Entities;

namespace Alumni.Persistence.Seed.Factories
{
    /// <summary>
    /// Announcement varlıkları için duyuru örnekleri üreten fabrika sınıfı.
    /// </summary>
    public static class AnnouncementFactory
    {
        public static Announcement Create(
            string title,
            string content,
            string author = "Kariyer Merkezi",
            DateTime? createdAt = null,
            bool isActive = true)
        {
            return new Announcement
            {
                Title = title,
                Content = content,
                Author = author,
                CreatedAt = createdAt ?? DateTime.UtcNow,
                IsActive = isActive
            };
        }

        public static List<Announcement> CreateDefaultAnnouncements()
        {
            return new List<Announcement>
            {
                Create(
                    title: "2026 Yıllık Mezunlar Buluşması ve Kariyer Günü",
                    content: "Tüm mezunlarımızı kampüsümüzde gerçekleşecek geleneksel mezunlar buluşmasına ve kariyer networking oturumlarına davet ediyoruz.",
                    author: "Kariyer Merkezi"
                ),
                Create(
                    title: "Mezun Takip Sistemi Yeni Veritabanı ve Portali Devrede",
                    content: "PostgreSQL ve EF Core altyapısı ile modernize edilen yeni Mezun Takip Sistemi yayına alınmıştır.",
                    author: "Rektörlük Bilgi İşlem"
                )
            };
        }
    }
}
