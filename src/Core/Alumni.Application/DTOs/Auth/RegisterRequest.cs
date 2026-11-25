namespace Alumni.Application.DTOs.Auth
{
    /// <summary>
    /// Kayıt (Sign Up / Register) isteği modeli.
    /// Hem /register MVC formu hem de POST /api/register REST uç noktası tarafından kullanılır.
    /// Güvenlik: İstemci Role alanına "Admin" gönderse dahi sunucu bunu yok sayar ve daima "Alumni" atar.
    /// </summary>
    public class RegisterRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }

        // İstemci tarafından gönderilse bile açık kayıtta daima yok sayılır (DATA_DESIGN.md §4.1 & §8.5)
        public string? Role { get; set; }

        // İsteğe bağlı mezun profil bilgileri (alumni_profiles)
        public string? Department { get; set; }
        public int? GraduationYear { get; set; }
        public string? CurrentCompany { get; set; }
        public string? CurrentTitle { get; set; }
    }
}
