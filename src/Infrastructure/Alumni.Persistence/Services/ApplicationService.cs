using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alumni.Application.DTOs.Applications;
using Alumni.Application.Interfaces;
using Alumni.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using ApplicationEntity = Alumni.Domain.Entities.Application;

namespace Alumni.Persistence.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly AlumniDbContext _context;

        public ApplicationService(AlumniDbContext context)
        {
            _context = context;
        }

        public async Task<ApplyResult> ApplyAsync(int jobId, int userId)
        {
            var jobExists = await _context.JobPosts.AnyAsync(j => j.Id == jobId);
            if (!jobExists)
            {
                return new ApplyResult(ApplyResultStatus.JobNotFound, null, $"ID {jobId} olan iş ilanı bulunamadı.");
            }

            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                return new ApplyResult(ApplyResultStatus.UserNotFound, null, $"ID {userId} olan kullanıcı bulunamadı.");
            }

            var alreadyApplied = await _context.Applications.AnyAsync(a => a.UserId == userId && a.JobPostId == jobId);
            if (alreadyApplied)
            {
                return new ApplyResult(ApplyResultStatus.AlreadyApplied, null, "Bu kullanıcı bu ilana daha önce başvuru yapmış.");
            }

            var application = new ApplicationEntity
            {
                UserId = userId,
                JobPostId = jobId,
                Status = "Applied",
                CreatedAt = DateTime.UtcNow
            };

            await _context.Applications.AddAsync(application);
            await _context.SaveChangesAsync();

            return new ApplyResult(ApplyResultStatus.Success, application, "Başvuru başarıyla oluşturuldu.");
        }

        public async Task<List<ApplicationEntity>> GetAllAsync()
        {
            return await _context.Applications
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<ApplicationEntity?> GetByIdAsync(int id)
        {
            return await _context.Applications
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<ApplicantDto>> GetApplicantsByJobIdAsync(int jobId)
        {
            return await _context.Applications
                .Include(a => a.User)
                .Where(a => a.JobPostId == jobId)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new ApplicantDto
                {
                    ApplicationId = a.Id,
                    UserId = a.UserId,
                    FirstName = a.User != null ? a.User.FirstName : string.Empty,
                    LastName = a.User != null ? a.User.LastName : string.Empty,
                    Email = a.User != null ? a.User.Email : string.Empty,
                    Status = a.Status,
                    AppliedAt = a.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<List<UserApplicationDto>> GetApplicationsByUserIdAsync(int userId)
        {
            return await _context.Applications
                .Include(a => a.JobPost)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new UserApplicationDto
                {
                    ApplicationId = a.Id,
                    JobPostId = a.JobPostId,
                    JobTitle = a.JobPost != null ? a.JobPost.Title : string.Empty,
                    Company = a.JobPost != null ? a.JobPost.Company : string.Empty,
                    Status = a.Status,
                    AppliedAt = a.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateStatusAsync(int id, string newStatus)
        {
            var application = await _context.Applications.FirstOrDefaultAsync(a => a.Id == id);
            if (application == null) return false;

            application.Status = newStatus;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var application = await _context.Applications.FirstOrDefaultAsync(a => a.Id == id);
            if (application == null) return false;

            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
