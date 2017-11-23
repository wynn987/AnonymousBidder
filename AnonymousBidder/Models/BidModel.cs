using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AnonymousBidder.Models
{
    public class BidModel
    {
        public Guid BidGUID { get; set; }
        [Range(0.0, 99999999)]
        public decimal BidPlaced { get; set; }
        public Guid Bid_AuctionGUID { get; set; }
        public Guid? Bid_ABUserGUID { get; set; }
    }
}