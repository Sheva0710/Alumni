namespace Alumni.Domain.Entities
{
    public class AlumniProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Department { get; set; } = string.Empty;
        public int GraduationYear { get; set; }
        public string? CurrentCompany { get; set; }
        public string? CurrentTitle { get; set; }
        public string? LinkedInUrl { get; set; }

        // Navigation Property (1-to-1)
        public User? User { get; set; }
    }
}
