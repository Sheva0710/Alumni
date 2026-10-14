using System.Collections.Generic;
using System.Threading.Tasks;
using Alumni.Domain.Entities;

namespace Alumni.Application.Interfaces
{
    public interface IUserService
    {
        List<User> GetAll();
        Task<List<User>> GetAllAsync();

        User? GetById(int id);
        Task<User?> GetByIdAsync(int id);

        User Add(User user);
        Task<User> AddAsync(User user);

        bool Update(int id, string? firstName, string? lastName, string? email, string? role);
        Task<bool> UpdateAsync(int id, string? firstName, string? lastName, string? email, string? role);

        bool Patch(int id, string? firstName, string? lastName, string? email, string? role);
        Task<bool> PatchAsync(int id, string? firstName, string? lastName, string? email, string? role);

        bool Delete(int id);
        Task<bool> DeleteAsync(int id);
    }
}
