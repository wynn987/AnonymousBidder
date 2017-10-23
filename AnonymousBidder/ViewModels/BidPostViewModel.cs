using AnonymousBidder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnonymousBidder.ViewModels
{
    public class BidPostViewModel
    {
        public AuctionModel AuctionModel { get; set; }
        public BidModel BidModel { get; set; }
        public FilePathModel ImageModel { get; set; }
    }
}