using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnonymousBidder.Models
{
    public class ABUserModel
    {
        public Guid ABUserGUID { get; set; }
        public string Alias { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public int Money { get; set; }

        public Guid ABUser_UserRoleGUID { get; set; }
        public Guid? ABUser_AuctionGUID { get; set; }
    }
}