using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AnonymousBidder.ViewModels
{
    public class BAccountCreateViewModel
    {
        //public ABUserModel ABUser { get; set; }
        public Guid auctionGUID { get; set; }
        

        [Required]
        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "The Email Address is invalid.")]
        public string EmailAddress { get; set; }


        [Required]
        [Display(Name = "Alias")]
        public string Alias { get; set; }

        [Required]
        [RegularExpression(@"((?=.*\d)(?=.*[A-Z])(?=.*[a-z])(?=.*\W).{8,16})", ErrorMessage = "The password is not strong enough.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [RegularExpression(@"((?=.*\d)(?=.*[A-Z])(?=.*[a-z])(?=.*\W).{8,16})", ErrorMessage = "The password is not strong enough.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }


        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Money to deposit")]
        public int Money { get; set; }


    }
}
