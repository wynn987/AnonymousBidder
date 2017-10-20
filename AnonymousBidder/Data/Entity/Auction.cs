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
        [Required, Range(0, 9999999999.99)]
        public decimal StartingBid { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public bool SellerSent { get; set; }
        [Required]
        public bool BuyerReceived { get; set; }
        [Required]
        public bool AuctionOver { get; set; }

        [ForeignKey("CurrentBid")]
        public Guid? Auction_BidGUID { get; set; }
        public virtual Bid CurrentBid { get; set; } // Current Bidder info
        public virtual ICollection<FilePath> AuctionImages { get; set; }
        public virtual ICollection<ABUser> AllUsers { get; set; }

        public Auction()
        {
            AuctionImages = new List<FilePath>();
            AllUsers = new List<ABUser>();
        }
    }
}