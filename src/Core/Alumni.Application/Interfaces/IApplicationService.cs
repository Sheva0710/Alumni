using System.Collections.Generic;
using System.Threading.Tasks;
using Alumni.Application.DTOs.Applications;
using ApplicationEntity = Alumni.Domain.Entities.Application;

namespace Alumni.Application.Interfaces
{
    public enum ApplyResultStatus
    {
        Success,
        JobNotFound,
        UserNotFound,
        AlreadyApplied
    }

    public record ApplyResult(ApplyResultStatus Status, ApplicationEntity? Application, string? Message = null);

    public interface IApplicationService
    {
        Task<ApplyResult> ApplyAsync(int jobId, int userId);
        Task<List<ApplicationEntity>> GetAllAsync();
        Task<ApplicationEntity?> GetByIdAsync(int id);
        Task<List<ApplicantDto>> GetApplicantsByJobIdAsync(int jobId);
        Task<List<UserApplicationDto>> GetApplicationsByUserIdAsync(int userId);
        Task<bool> UpdateStatusAsync(int id, string newStatus);
        Task<bool> DeleteAsync(int id);
    }
}
