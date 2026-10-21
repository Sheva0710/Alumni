using System;

namespace Alumni.Application.DTOs.Applications
{
    public class ApplicantDto
    {
        public int ApplicationId { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Status { get; set; } = "Applied";
        public DateTime AppliedAt { get; set; }
    }
}
