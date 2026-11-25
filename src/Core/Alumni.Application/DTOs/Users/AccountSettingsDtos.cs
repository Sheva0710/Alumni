namespace Alumni.Application.DTOs.Users
{
    public class UpdateProfileRequest
    {
        public string? Department { get; set; }
        public int? GraduationYear { get; set; }
        public string? CurrentCompany { get; set; }
        public string? CurrentTitle { get; set; }
        public string? LinkedInUrl { get; set; }
    }

    public class UpdateAccountRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmNewPassword { get; set; }
    }

    public class AccountOperationResult
    {
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
        public Domain.Entities.User? User { get; set; }

        public static AccountOperationResult Ok(string message, Domain.Entities.User? user = null) =>
            new() { Succeeded = true, Message = message, User = user };

        public static AccountOperationResult Fail(string message) =>
            new() { Succeeded = false, Message = message };
    }
}
