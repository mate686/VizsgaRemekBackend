using System.ComponentModel.DataAnnotations;

namespace VizsgaRemekBackend.Dtos.AuthDtos
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
