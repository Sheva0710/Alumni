using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alumni.Application.Interfaces;
using Alumni.Domain.Entities;

namespace Alumni.Application.Services
{
    public class AnnouncementService : IAnnouncementService
    {
        private static readonly object _lock = new();
        private static readonly List<Announcement> _announcements = new()
        {
            new Announcement
            {
                Id = 1,
                Title = "Bahar Dönemi Mezunlar Buluşması 2026",
                Content = "Üniversitemiz mezunları ile öğrencilerimizi bir araya getiren geleneksel bahar buluşması 15 Mayıs'ta ana kampüste gerçekleşecektir.",
                Author = "Kariyer Merkezi",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                IsActive = true
            },
            new Announcement
            {
                Id = 2,
                Title = "Yapay Zekâ ve Yazılım Sektörü Mentorluk Programı",
                Content = "Tecrübeli mezunlarımız ile son sınıf mühendislik öğrencilerimizi eşleştiren 8 haftalık mentorluk başvuruları başlamıştır.",
                Author = "Mezunlar Ofisi",
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                IsActive = true
            }
        };

        public List<Announcement> GetAll()
        {
            lock (_lock)
            {
                return _announcements.OrderByDescending(a => a.CreatedAt).ToList();
            }
        }

        public Task<List<Announcement>> GetAllAsync()
        {
            return Task.FromResult(GetAll());
        }

        public Announcement? GetById(int id)
        {
            lock (_lock)
            {
                return _announcements.FirstOrDefault(a => a.Id == id);
            }
        }

        public Task<Announcement?> GetByIdAsync(int id)
        {
            return Task.FromResult(GetById(id));
        }

        public Announcement Add(Announcement announcement)
        {
            lock (_lock)
            {
                announcement.Id = _announcements.Count > 0 ? _announcements.Max(a => a.Id) + 1 : 1;
                announcement.CreatedAt = DateTime.UtcNow;
                _announcements.Add(announcement);
                return announcement;
            }
        }

        public Task<Announcement> AddAsync(Announcement announcement)
        {
            return Task.FromResult(Add(announcement));
        }

        public bool Update(int id, string title, string content, string? author, bool isActive)
        {
            lock (_lock)
            {
                var item = _announcements.FirstOrDefault(a => a.Id == id);
                if (item == null) return false;

                item.Title = title;
                item.Content = content;
                if (!string.IsNullOrWhiteSpace(author)) item.Author = author;
                item.IsActive = isActive;
                return true;
            }
        }

        public Task<bool> UpdateAsync(int id, string title, string content, string? author, bool isActive)
        {
            return Task.FromResult(Update(id, title, content, author, isActive));
        }

        public bool Delete(int id)
        {
            lock (_lock)
            {
                var item = _announcements.FirstOrDefault(a => a.Id == id);
                if (item == null) return false;

                _announcements.Remove(item);
                return true;
            }
        }

        public Task<bool> DeleteAsync(int id)
        {
            return Task.FromResult(Delete(id));
        }
    }
}
