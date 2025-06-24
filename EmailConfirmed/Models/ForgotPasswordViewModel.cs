using System.ComponentModel.DataAnnotations;

namespace EmailConfirmed.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
