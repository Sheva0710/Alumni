using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alumni.Application.Interfaces;
using Alumni.Domain.Entities;
using Alumni.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Alumni.Persistence.Services
{
    /// <summary>
    /// PostgreSQL veritabanı (AlumniDbContext) üzerinden Announcement CRUD işlemlerini yürüten servis.
    /// </summary>
    public class AnnouncementService : IAnnouncementService
    {
        private readonly AlumniDbContext _context;

        public AnnouncementService(AlumniDbContext context)
        {
            _context = context;
        }

        public async Task<List<Announcement>> GetAllAsync()
        {
            return await _context.Announcements
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Announcement?> GetByIdAsync(int id)
        {
            return await _context.Announcements.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Announcement> AddAsync(Announcement announcement)
        {
            announcement.CreatedAt = announcement.CreatedAt == default ? DateTime.UtcNow : announcement.CreatedAt;
            await _context.Announcements.AddAsync(announcement);
            await _context.SaveChangesAsync();
            return announcement;
        }

        public async Task<bool> UpdateAsync(int id, string title, string content, string? author, bool isActive)
        {
            var item = await _context.Announcements.FirstOrDefaultAsync(a => a.Id == id);
            if (item == null) return false;

            item.Title = title;
            item.Content = content;
            if (!string.IsNullOrWhiteSpace(author)) item.Author = author;
            item.IsActive = isActive;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PatchAsync(int id, string? title, string? content, string? author, bool? isActive)
        {
            var item = await _context.Announcements.FirstOrDefaultAsync(a => a.Id == id);
            if (item == null) return false;

            if (!string.IsNullOrWhiteSpace(title)) item.Title = title;
            if (!string.IsNullOrWhiteSpace(content)) item.Content = content;
            if (!string.IsNullOrWhiteSpace(author)) item.Author = author;
            if (isActive.HasValue) item.IsActive = isActive.Value;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _context.Announcements.FirstOrDefaultAsync(a => a.Id == id);
            if (item == null) return false;

            _context.Announcements.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

