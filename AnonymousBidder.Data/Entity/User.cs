using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AnonymousBidder.Data.Entity
{
    public class User : BaseEntity
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid UserGUID { get; set; }
        [Required]
        public string Alias { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string Token { get; set; }

        [Required]
        public Guid User_RoleGUID { get; set; }
        public Guid? User_AuctionGUID { get; set; }
        [ForeignKey("User_RoleGUID")]
        public virtual Role Role { get; set; }
        [ForeignKey("User_AuctionGUID")]
        public virtual Auction Auction { get; set; }
    }
}