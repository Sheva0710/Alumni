using System.Collections.Generic;
using System.Threading.Tasks;
using Alumni.Domain.Entities;

namespace Alumni.Application.Interfaces
{
    public interface IAnnouncementService
    {
        Task<List<Announcement>> GetAllAsync();
        Task<Announcement?> GetByIdAsync(int id);
        Task<Announcement> AddAsync(Announcement announcement);
        Task<bool> UpdateAsync(int id, string title, string content, string? author, bool isActive);
        Task<bool> PatchAsync(int id, string? title, string? content, string? author, bool? isActive);
        Task<bool> DeleteAsync(int id);
    }
}

