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
    public class JobPostService : IJobPostService
    {
        private readonly AlumniDbContext _context;

        public JobPostService(AlumniDbContext context)
        {
            _context = context;
        }

        public async Task<List<JobPost>> GetAllAsync()
        {
            return await _context.JobPosts
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
        }

        public async Task<JobPost?> GetByIdAsync(int id)
        {
            return await _context.JobPosts
                .FirstOrDefaultAsync(j => j.Id == id);
        }

        public async Task<List<JobPost>> GetByUserIdAsync(int userId)
        {
            return await _context.JobPosts
                .Where(j => j.PostedByUserId == userId)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
        }

        public async Task<JobPost?> CreateForUserAsync(int userId, string title, string company, string description)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                return null;
            }

            var jobPost = new JobPost
            {
                PostedByUserId = userId,
                Title = title.Trim(),
                Company = company.Trim(),
                Description = description.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await _context.JobPosts.AddAsync(jobPost);
            await _context.SaveChangesAsync();
            return jobPost;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var job = await _context.JobPosts.FirstOrDefaultAsync(j => j.Id == id);
            if (job == null) return false;

            _context.JobPosts.Remove(job);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
