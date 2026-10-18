using System.Collections.Generic;
using System.Threading.Tasks;
using Alumni.Domain.Entities;

namespace Alumni.Application.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User> AddAsync(User user);
        Task<bool> UpdateAsync(int id, string? firstName, string? lastName, string? email, string? role);
        Task<bool> PatchAsync(int id, string? firstName, string? lastName, string? email, string? role);
        Task<bool> DeleteAsync(int id);
    }
}

