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
    /// PostgreSQL veritabanı (AlumniDbContext) üzerinden User CRUD işlemlerini yürüten servis.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly AlumniDbContext _context;

        public UserService(AlumniDbContext context)
        {
            _context = context;
        }

        public List<User> GetAll()
        {
            return _context.Users
                .Include(u => u.Profile)
                .OrderBy(u => u.Id)
                .ToList();
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.Profile)
                .OrderBy(u => u.Id)
                .ToListAsync();
        }

        public User? GetById(int id)
        {
            return _context.Users
                .Include(u => u.Profile)
                .Include(u => u.JobPosts)
                .FirstOrDefault(u => u.Id == id);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Profile)
                .Include(u => u.JobPosts)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public User Add(User user)
        {
            user.CreatedAt = user.CreatedAt == default ? DateTime.UtcNow : user.CreatedAt;
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        public async Task<User> AddAsync(User user)
        {
            user.CreatedAt = user.CreatedAt == default ? DateTime.UtcNow : user.CreatedAt;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public bool Update(int id, string? firstName, string? lastName, string? email, string? role)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return false;

            if (!string.IsNullOrWhiteSpace(firstName)) user.FirstName = firstName;
            if (!string.IsNullOrWhiteSpace(lastName)) user.LastName = lastName;
            if (!string.IsNullOrWhiteSpace(email)) user.Email = email.Trim().ToLowerInvariant();
            if (!string.IsNullOrWhiteSpace(role)) user.Role = role;

            _context.SaveChanges();
            return true;
        }

        public async Task<bool> UpdateAsync(int id, string? firstName, string? lastName, string? email, string? role)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return false;

            if (!string.IsNullOrWhiteSpace(firstName)) user.FirstName = firstName;
            if (!string.IsNullOrWhiteSpace(lastName)) user.LastName = lastName;
            if (!string.IsNullOrWhiteSpace(email)) user.Email = email.Trim().ToLowerInvariant();
            if (!string.IsNullOrWhiteSpace(role)) user.Role = role;

            await _context.SaveChangesAsync();
            return true;
        }

        public bool Patch(int id, string? firstName, string? lastName, string? email, string? role)
        {
            return Update(id, firstName, lastName, email, role);
        }

        public async Task<bool> PatchAsync(int id, string? firstName, string? lastName, string? email, string? role)
        {
            return await UpdateAsync(id, firstName, lastName, email, role);
        }

        public bool Delete(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return false;

            _context.Users.Remove(user);
            _context.SaveChanges();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
