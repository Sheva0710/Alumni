using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Alumni.Application.DTOs.Auth;
using Alumni.Application.DTOs.Users;
using Alumni.Application.Interfaces;
using Alumni.Domain.Entities;
using Alumni.Persistence.Data;
using Alumni.Persistence.Seed.Factories;
using Microsoft.EntityFrameworkCore;

namespace Alumni.Persistence.Services
{
    /// <summary>
    /// PostgreSQL veritabanı (AlumniDbContext) üzerinden User CRUD ve kayıt işlemlerini yürüten servis.
    /// </summary>
    public class UserService : IUserService
    {
        private static readonly Regex BasicEmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
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

        public async Task<User?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            var normalizedEmail = email.Trim().ToLowerInvariant();
            return await _context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Email == normalizedEmail);
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            var user = await GetByEmailAsync(email);
            if (user == null || string.IsNullOrWhiteSpace(user.PasswordHash))
                return null;

            try
            {
                // BCrypt, girilen şifreyi kayıtlı salt ile özetleyerek doğrular (asla == kullanılmaz).
                var isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
                return isValid ? user : null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<User> AddAsync(User user)
        {
            user.Email = user.Email.Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                user.PasswordHash = UserFactory.HashPassword(UserFactory.DefaultSeedPassword);
            }

            user.CreatedAt = user.CreatedAt == default ? DateTime.UtcNow : user.CreatedAt;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<RegisterUserResult> RegisterAsync(RegisterRequest request, bool requireConfirmPassword = false)
        {
            var firstName = request.FirstName?.Trim() ?? string.Empty;
            var lastName = request.LastName?.Trim() ?? string.Empty;
            var email = request.Email?.Trim().ToLowerInvariant() ?? string.Empty;
            var password = request.Password ?? string.Empty;

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            {
                return new RegisterUserResult
                {
                    Status = RegisterUserStatus.ValidationError,
                    ErrorMessage = "Ad ve soyad alanları zorunludur."
                };
            }

            if (firstName.Length > 100 || lastName.Length > 100)
            {
                return new RegisterUserResult
                {
                    Status = RegisterUserStatus.ValidationError,
                    ErrorMessage = "Ad ve soyad en fazla 100 karakter olabilir."
                };
            }

            if (string.IsNullOrWhiteSpace(email) || !BasicEmailRegex.IsMatch(email))
            {
                return new RegisterUserResult
                {
                    Status = RegisterUserStatus.ValidationError,
                    ErrorMessage = "Lütfen geçerli bir e-posta adresi giriniz."
                };
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                return new RegisterUserResult
                {
                    Status = RegisterUserStatus.ValidationError,
                    ErrorMessage = "Şifre zorunludur ve en az 6 karakter olmalıdır."
                };
            }

            if (requireConfirmPassword && !string.Equals(password, request.ConfirmPassword, StringComparison.Ordinal))
            {
                return new RegisterUserResult
                {
                    Status = RegisterUserStatus.ValidationError,
                    ErrorMessage = "Şifre ve şifre tekrarı birbiriyle eşleşmiyor."
                };
            }

            // İsteğe bağlı mezun profil alanları kontrolü
            var hasProfileData = !string.IsNullOrWhiteSpace(request.Department) ||
                                 request.GraduationYear.HasValue ||
                                 !string.IsNullOrWhiteSpace(request.CurrentCompany) ||
                                 !string.IsNullOrWhiteSpace(request.CurrentTitle);

            if (hasProfileData)
            {
                if (string.IsNullOrWhiteSpace(request.Department))
                {
                    return new RegisterUserResult
                    {
                        Status = RegisterUserStatus.ValidationError,
                        ErrorMessage = "Mezun profil bilgisi ekliyorsanız bölüm alanı zorunludur."
                    };
                }

                if (!request.GraduationYear.HasValue || request.GraduationYear < 1950 || request.GraduationYear > 2100)
                {
                    return new RegisterUserResult
                    {
                        Status = RegisterUserStatus.ValidationError,
                        ErrorMessage = "Mezuniyet yılı 1950 ile 2100 arasında olmalıdır."
                    };
                }
            }

            var emailExists = await _context.Users.AnyAsync(u => u.Email == email);
            if (emailExists)
            {
                return new RegisterUserResult
                {
                    Status = RegisterUserStatus.EmailAlreadyExists,
                    ErrorMessage = "Bu e-posta adresiyle kayıtlı bir hesap zaten mevcut."
                };
            }

            // DATA_DESIGN.md: Açık kayıtta rol her zaman "Alumni" (graduate) olarak kilitlenir.
            // İstemci "role": "Admin" gönderse bile yok sayılır.
            var newUser = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash = UserFactory.HashPassword(password),
                Role = "Alumni",
                CreatedAt = DateTime.UtcNow
            };

            if (hasProfileData)
            {
                newUser.Profile = new AlumniProfile
                {
                    Department = request.Department!.Trim(),
                    GraduationYear = request.GraduationYear!.Value,
                    CurrentCompany = string.IsNullOrWhiteSpace(request.CurrentCompany) ? null : request.CurrentCompany.Trim(),
                    CurrentTitle = string.IsNullOrWhiteSpace(request.CurrentTitle) ? null : request.CurrentTitle.Trim()
                };
            }

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return new RegisterUserResult
            {
                Status = RegisterUserStatus.Success,
                User = newUser
            };
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

        public async Task<AccountOperationResult> UpsertProfileAsync(int userId, UpdateProfileRequest request)
        {
            var user = await _context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return AccountOperationResult.Fail("Kullanıcı bulunamadı.");
            }

            var department = request.Department?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(department))
            {
                return AccountOperationResult.Fail("Bölüm alanı zorunludur.");
            }

            if (department.Length > 150)
            {
                return AccountOperationResult.Fail("Bölüm adı en fazla 150 karakter olabilir.");
            }

            if (!request.GraduationYear.HasValue || request.GraduationYear < 1950 || request.GraduationYear > 2100)
            {
                return AccountOperationResult.Fail("Mezuniyet yılı 1950 ile 2100 arasında olmalıdır.");
            }

            var linkedIn = string.IsNullOrWhiteSpace(request.LinkedInUrl) ? null : request.LinkedInUrl.Trim();
            if (linkedIn != null)
            {
                var validLinkedIn = linkedIn.StartsWith("https://linkedin.com/in/", StringComparison.OrdinalIgnoreCase) ||
                                    linkedIn.StartsWith("https://www.linkedin.com/in/", StringComparison.OrdinalIgnoreCase);
                if (!validLinkedIn)
                {
                    return AccountOperationResult.Fail("LinkedIn bağlantısı 'https://linkedin.com/in/...' veya 'https://www.linkedin.com/in/...' ile başlamalıdır.");
                }
            }

            if (user.Profile == null)
            {
                user.Profile = new AlumniProfile
                {
                    UserId = user.Id,
                    Department = department,
                    GraduationYear = request.GraduationYear.Value,
                    CurrentCompany = string.IsNullOrWhiteSpace(request.CurrentCompany) ? null : request.CurrentCompany.Trim(),
                    CurrentTitle = string.IsNullOrWhiteSpace(request.CurrentTitle) ? null : request.CurrentTitle.Trim(),
                    LinkedInUrl = linkedIn
                };
            }
            else
            {
                user.Profile.Department = department;
                user.Profile.GraduationYear = request.GraduationYear.Value;
                user.Profile.CurrentCompany = string.IsNullOrWhiteSpace(request.CurrentCompany) ? null : request.CurrentCompany.Trim();
                user.Profile.CurrentTitle = string.IsNullOrWhiteSpace(request.CurrentTitle) ? null : request.CurrentTitle.Trim();
                user.Profile.LinkedInUrl = linkedIn;
            }

            await _context.SaveChangesAsync();
            return AccountOperationResult.Ok("Mezun profil bilgileriniz başarıyla güncellendi.", user);
        }

        public async Task<AccountOperationResult> UpdateOwnAccountAsync(int userId, UpdateAccountRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return AccountOperationResult.Fail("Kullanıcı bulunamadı.");
            }

            var firstName = request.FirstName?.Trim() ?? string.Empty;
            var lastName = request.LastName?.Trim() ?? string.Empty;
            var email = request.Email?.Trim().ToLowerInvariant() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            {
                return AccountOperationResult.Fail("Ad ve soyad alanları zorunludur.");
            }

            if (firstName.Length > 100 || lastName.Length > 100)
            {
                return AccountOperationResult.Fail("Ad ve soyad en fazla 100 karakter olabilir.");
            }

            if (string.IsNullOrWhiteSpace(email) || !BasicEmailRegex.IsMatch(email))
            {
                return AccountOperationResult.Fail("Lütfen geçerli bir e-posta adresi giriniz.");
            }

            var emailTakenByOther = await _context.Users.AnyAsync(u => u.Email == email && u.Id != userId);
            if (emailTakenByOther)
            {
                return AccountOperationResult.Fail("Bu e-posta adresi başka bir kullanıcı tarafından kullanılıyor.");
            }

            user.FirstName = firstName;
            user.LastName = lastName;
            user.Email = email;

            await _context.SaveChangesAsync();
            return AccountOperationResult.Ok("Hesap bilgileriniz başarıyla güncellendi.", user);
        }

        public async Task<AccountOperationResult> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return AccountOperationResult.Fail("Kullanıcı bulunamadı.");
            }

            if (string.IsNullOrWhiteSpace(request.CurrentPassword))
            {
                return AccountOperationResult.Fail("Mevcut şifrenizi girmelisiniz.");
            }

            var currentValid = !string.IsNullOrWhiteSpace(user.PasswordHash) &&
                               BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash);
            if (!currentValid)
            {
                return AccountOperationResult.Fail("Mevcut şifreniz hatalı.");
            }

            if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 6)
            {
                return AccountOperationResult.Fail("Yeni şifre en az 6 karakter olmalıdır.");
            }

            if (!string.Equals(request.NewPassword, request.ConfirmNewPassword, StringComparison.Ordinal))
            {
                return AccountOperationResult.Fail("Yeni şifre ve tekrarı eşleşmiyor.");
            }

            user.PasswordHash = UserFactory.HashPassword(request.NewPassword);
            await _context.SaveChangesAsync();

            return AccountOperationResult.Ok("Şifreniz güvenli bir şekilde güncellendi.", user);
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

