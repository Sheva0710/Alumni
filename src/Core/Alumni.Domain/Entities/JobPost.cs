namespace Alumni.Domain.Entities
{
    public class JobPost
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int PostedByUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Property (1-to-N)
        public User? PostedByUser { get; set; }
    }
}
