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
        [Required, RegularExpression(@"^\d+\.\d{0,2}$"), Range(0, 9999999999.99)]
        public decimal BidPlaced { get; set; }
        [Required]
        [Key, ForeignKey("Auction")]
        public Guid Bid_AuctionGUID{ get; set; }
        public Guid? Bid_UserGUID { get; set; }

        public virtual Auction Auction { get; set; }
        [ForeignKey("Bid_UserGUID")]
        public virtual User User { get; set; }
    }
}