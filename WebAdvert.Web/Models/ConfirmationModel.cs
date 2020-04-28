using System.ComponentModel.DataAnnotations;

namespace WebAdvert.Web.Models
{
    public class ConfirmationModel
    {
        [Required(ErrorMessage = "Email is Required.")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Code is Required.")]
        [Display(Name = "Code")]
        public string Code { get; set; }
    }
}
