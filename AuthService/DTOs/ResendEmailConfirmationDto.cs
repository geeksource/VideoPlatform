using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs
{
    public class ResendEmailConfirmationDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

}
