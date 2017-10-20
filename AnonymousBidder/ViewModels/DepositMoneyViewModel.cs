using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace AnonymousBidder.ViewModels
{
    public class DepositMoneyViewModel
    {

        [Required]
        [Display(Name = "Money")]
        public int Money { get; set; }

    }
}