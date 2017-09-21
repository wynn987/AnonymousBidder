using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnonymousBidder.Models
{
    public class BidModel
    {
        public Guid BidGUID { get; set; }
        public decimal BidPlaced { get; set; }
        public Guid Bid_AuctionGUID { get; set; }
        public Guid? Bid_ABUserGUID { get; set; }
    }
}