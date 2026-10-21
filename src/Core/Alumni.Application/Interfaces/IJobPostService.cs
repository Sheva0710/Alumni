using System.Collections.Generic;
using System.Threading.Tasks;
using Alumni.Domain.Entities;

namespace Alumni.Application.Interfaces
{
    public interface IJobPostService
    {
        Task<List<JobPost>> GetAllAsync();
        Task<JobPost?> GetByIdAsync(int id);
        Task<List<JobPost>> GetByUserIdAsync(int userId);
        Task<JobPost?> CreateForUserAsync(int userId, string title, string company, string description);
        Task<bool> DeleteAsync(int id);
    }
}
