﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace AnonymousBidder.ViewModels
{
    public class MAccountCreateViewModel
    {
        public Guid userGUID { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "The Email Address is invalid.")]
        public string EmailAddress { get; set; }


        [Required]
        [Display(Name = "Alias")]
        public string Alias { get; set; }

        [Required]
        // [RegularExpression(@"((?=.*\d)(?=.*[A-Z])(?=.*[a-z])(?=.*\W).{8,16})", ErrorMessage = "The password is not strong enough.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }
}