namespace Alumni.Application.DTOs.Auth
{
    /// <summary>
    /// Giriş (Login) isteği modeli.
    /// Hem POST /api/login hem de POST /login MVC formu tarafından kullanılır.
    /// </summary>
    public class LoginRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
