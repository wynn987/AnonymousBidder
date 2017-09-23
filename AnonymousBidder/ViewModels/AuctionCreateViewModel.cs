using AnonymousBidder.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;

namespace AnonymousBidder.ViewModels
{
    public class AuctionCreateViewModel
    {
        public AuctionModel Auction { get; set; }
        public IEnumerable<HttpPostedFileBase> Files { get; set; }
        public ABUserModel Seller { get; set; }
    }
}
