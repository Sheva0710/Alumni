namespace Alumni.Application.DTOs.Announcements
{
    public class PatchAnnouncementRequest
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Author { get; set; }
        public bool? IsActive { get; set; }
    }
}
