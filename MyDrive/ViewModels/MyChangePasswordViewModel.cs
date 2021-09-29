using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyDrive.ViewModels
{
    public class MyChangePasswordViewModel
    {
        [Required(ErrorMessage = "Old Password Is Required")]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; }

        
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public Boolean Flag { get; set; }

        public Boolean Flag2 { get; set; }
    }
}