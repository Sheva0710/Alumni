using Alumni.Domain.Entities;

namespace Alumni.Application.Interfaces
{
    public interface IAnnouncementService
    {
        List<Announcement> GetAll();
        Announcement? GetById(int id);
        Announcement Add(Announcement announcement);
        bool Update(int id, string title, string content, string? author, bool isActive);
        bool Delete(int id);
    }
}
