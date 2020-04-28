using System.ComponentModel.DataAnnotations;

namespace WebAdvert.Web.Models
{
    public class SignupModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Emaill")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(8, ErrorMessage = "Password must be 8 characers long.")]
        public string Pasword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
