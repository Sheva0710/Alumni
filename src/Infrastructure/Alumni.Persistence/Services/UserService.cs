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

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.Profile)
                .OrderBy(u => u.Id)
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Profile)
                .Include(u => u.JobPosts)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> AddAsync(User user)
        {
            user.CreatedAt = user.CreatedAt == default ? DateTime.UtcNow : user.CreatedAt;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
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

        public async Task<bool> PatchAsync(int id, string? firstName, string? lastName, string? email, string? role)
        {
            return await UpdateAsync(id, firstName, lastName, email, role);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await DeleteUserWithRuleAsync(id);
            return result == DeleteUserResult.Success;
        }

        public async Task<DeleteUserResult> DeleteUserWithRuleAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return DeleteUserResult.NotFound;

            var hasJobPosts = await _context.JobPosts.AnyAsync(j => j.PostedByUserId == id);
            if (hasJobPosts)
            {
                return DeleteUserResult.HasJobPosts;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return DeleteUserResult.Success;
        }
    }
}

