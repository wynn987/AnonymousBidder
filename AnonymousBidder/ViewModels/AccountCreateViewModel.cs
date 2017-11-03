using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AnonymousBidder.ViewModels
{
    public class AccountCreateViewModel
    {
        //public ABUserModel ABUser { get; set; }
        public Guid userGUID { get; set; }
        public string userToken { get; set; }
        
        [Required]
        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "The Email Address is invalid.")]
        public string EmailAddress { get; set; }
        

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]{6,10}$", ErrorMessage = "The alias cannot contain symbols and should contain 6 to 10 characters.")]
        [Display(Name = "Alias")]
        public string Alias { get; set; }

        [Required]
        [RegularExpression(@"((?=.*\d)(?=.*[A-Z])(?=.*[a-z])(?=.*\W).{8,16})", ErrorMessage = "The password is not strong enough.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [RegularExpression(@"((?=.*\d)(?=.*[A-Z])(?=.*[a-z])(?=.*\W).{8,16})", ErrorMessage = "The password is not strong enough.")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }
}
