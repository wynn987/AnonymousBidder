using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AnonymousBidder.Data.Entity
{
    public class Bid : BaseEntity
    {
        [Required]
        public Guid BidGUID { get; set; }
        [Required, Range(0, 9999999999.99)]
        public decimal BidPlaced { get; set; }
        [Required]
        [Key, ForeignKey("Auction")]
        public Guid Bid_AuctionGUID{ get; set; }
        public Guid? Bid_ABUserGUID { get; set; }

        public virtual Auction Auction { get; set; }
        [ForeignKey("Bid_ABUserGUID")]
        public virtual ABUser User { get; set; }
    }
}