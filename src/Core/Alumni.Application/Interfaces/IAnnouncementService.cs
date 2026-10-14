using System.Collections.Generic;
using System.Threading.Tasks;
using Alumni.Domain.Entities;

namespace Alumni.Application.Interfaces
{
    public interface IAnnouncementService
    {
        List<Announcement> GetAll();
        Task<List<Announcement>> GetAllAsync();

        Announcement? GetById(int id);
        Task<Announcement?> GetByIdAsync(int id);

        Announcement Add(Announcement announcement);
        Task<Announcement> AddAsync(Announcement announcement);

        bool Update(int id, string title, string content, string? author, bool isActive);
        Task<bool> UpdateAsync(int id, string title, string content, string? author, bool isActive);

        bool Delete(int id);
        Task<bool> DeleteAsync(int id);
    }
}
