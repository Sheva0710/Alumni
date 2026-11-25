using Alumni.Application.DTOs.Applications;
using Alumni.Domain.Entities;

namespace Alumni.API.Common
{
    public class UserProfileViewModel
    {
        public User User { get; set; } = null!;
        public List<JobPost> JobPosts { get; set; } = new();
        public List<UserApplicationDto> Applications { get; set; } = new();

        public string AcademicStatus
        {
            get
            {
                if (User.Profile == null) return "Profil Tamamlanmadı";
                return User.Profile.GraduationYear > DateTime.UtcNow.Year
                    ? "Öğrenci / Aday Mezun"
                    : "Mezun";
            }
        }
    }
}
