namespace Alumni.Application.DTOs.Announcements
{
    public class UpdateAnnouncementRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Author { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
