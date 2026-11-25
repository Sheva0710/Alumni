using System.Collections.Generic;
using System.Threading.Tasks;
using Alumni.Application.DTOs.Auth;
using Alumni.Application.DTOs.Users;
using Alumni.Domain.Entities;

namespace Alumni.Application.Interfaces
{
    public enum DeleteUserResult
    {
        Success,
        NotFound,
        HasJobPosts // DATA_DESIGN.md Restrict Rule -> 409 Conflict
    }

    public enum RegisterUserStatus
    {
        Success,
        ValidationError,
        EmailAlreadyExists // 409 Conflict
    }

    public class RegisterUserResult
    {
        public RegisterUserStatus Status { get; set; }
        public string? ErrorMessage { get; set; }
        public User? User { get; set; }
    }

    public interface IUserService
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> AuthenticateAsync(string email, string password);
        Task<User> AddAsync(User user);
        Task<RegisterUserResult> RegisterAsync(RegisterRequest request, bool requireConfirmPassword = false);
        Task<bool> UpdateAsync(int id, string? firstName, string? lastName, string? email, string? role);
        Task<bool> PatchAsync(int id, string? firstName, string? lastName, string? email, string? role);
        Task<AccountOperationResult> UpsertProfileAsync(int userId, UpdateProfileRequest request);
        Task<AccountOperationResult> UpdateOwnAccountAsync(int userId, UpdateAccountRequest request);
        Task<AccountOperationResult> ChangePasswordAsync(int userId, ChangePasswordRequest request);
        Task<bool> DeleteAsync(int id);
        Task<DeleteUserResult> DeleteUserWithRuleAsync(int id);
    }
}
