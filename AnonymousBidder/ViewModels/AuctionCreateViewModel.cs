using AnonymousBidder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnonymousBidder.ViewModels
{
    public class AuctionCreateViewModel
    {
        public AuctionModel Auction { get; set; }
        public IEnumerable<HttpPostedFileBase> Files { get; set; }
        public string CurrentDateStr { get; set; }
    }
}
