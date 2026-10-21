using System;

namespace Alumni.Application.DTOs.Applications
{
    public class UserApplicationDto
    {
        public int ApplicationId { get; set; }
        public int JobPostId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Status { get; set; } = "Applied";
        public DateTime AppliedAt { get; set; }
    }
}
