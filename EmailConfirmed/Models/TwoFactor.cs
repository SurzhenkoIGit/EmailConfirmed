using System.ComponentModel.DataAnnotations;

namespace EmailConfirmed.Models
{
    public class TwoFactor
    {
        [Required]
        public string TwoFactorCode { get; set; } = "";
        public string ReturnUrl { get; set; } = "/";
    }
}
