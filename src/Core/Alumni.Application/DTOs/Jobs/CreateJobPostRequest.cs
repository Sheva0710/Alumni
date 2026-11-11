using System.ComponentModel.DataAnnotations;

namespace Alumni.Application.DTOs.Jobs
{
    public class CreateJobPostRequest
    {
        [Required(ErrorMessage = "İlan başlığı (title) zorunludur.")]
        [MaxLength(120, ErrorMessage = "İlan başlığı (title) en fazla 120 karakter olabilir.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Firma adı (company) zorunludur.")]
        public string? Company { get; set; }

        [MaxLength(100, ErrorMessage = "Şehir bilgisi (city) en fazla 100 karakter olabilir.")]
        public string? City { get; set; }

        public string? Description { get; set; }
    }
}
