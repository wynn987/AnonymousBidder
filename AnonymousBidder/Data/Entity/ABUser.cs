using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AnonymousBidder.Data.Entity
{
    public class ABUser : BaseEntity
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid ABUserGUID { get; set; }
        [Required]
        public string Alias { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string Token { get; set; }

        [Required]
        public Guid ABUser_UserRoleGUID { get; set; }
        public Guid? ABUser_AuctionGUID { get; set; }
        [ForeignKey("ABUser_UserRoleGUID")]
        public virtual UserRole Role { get; set; }
        [ForeignKey("ABUser_AuctionGUID")]
        public virtual Auction Auction { get; set; }
    }
}