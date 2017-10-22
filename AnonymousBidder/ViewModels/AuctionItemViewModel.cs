using AnonymousBidder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnonymousBidder.ViewModels
{
    public class AuctionItemViewModel
    {
        public AuctionModel auctionItem { get; set; }
        public ABUserModel bidderInfo { get; set; }
        public BidModel bidInfo { get; set; }
    }
}