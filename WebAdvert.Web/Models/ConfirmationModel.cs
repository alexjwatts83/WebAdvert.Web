using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAdvert.Web.Models
{
    public class ConfirmationModel
    {
        [Required(ErrorMessage = "Email is Required.")]
        [EmailAddress]
        [Display(Name = "Emaill")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Code is Required.")]
        [Display(Name = "Code")]
        public string Code { get; set; }
    }
}
