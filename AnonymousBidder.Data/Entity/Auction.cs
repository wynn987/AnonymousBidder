using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AnonymousBidder.Data.Entity
{
    public class Auction : BaseEntity
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid AuctionGUID { get; set; }
        [MinLength(1), MaxLength(50), Required]
        public string ItemName { get; set; }
        [Required, RegularExpression(@"^\d+\.\d{0,2}$"), Range(0, 9999999999.99)]
        public decimal StartingBid { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }

        public Guid? Auction_BidGUID { get; set; }
        [ForeignKey("Auction_BidGUID")]
        public virtual Bid Bid { get; set; } // Current Bidder info
        public virtual ICollection<FilePath> AuctionImages { get; set; }
        public virtual ICollection<User> Users { get; set; }

        public Auction()
        {
            AuctionImages = new List<FilePath>();
            Users = new List<User>();
        }
    }
}