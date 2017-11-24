using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AnonymousBidder.Models
{
    public class AuctionModel
    {
        public Guid AuctionGUID { get; set; }
        [DisplayName("Item Name")]
        public string ItemName { get; set; }
        [Range(0.0, 99999999)]
        [DisplayName("Starting Bid")]
        public decimal StartingBid { get; set; }
        [DisplayName("Start Date and Time")]
        public DateTime StartDate { get; set; }
        [DisplayName("End Date and Time")]
        public DateTime EndDate { get; set; }
        public bool SellerSent { get; set; }
        public bool BuyerReceived { get; set; }
        public bool AuctionOver { get; set; }

        public Guid? Auction_BidGUID { get; set; }
        public ICollection<FilePathModel> AuctionImages { get; set; }
        public ICollection<ABUserModel> ABUsers { get; set; }

        public AuctionModel()
        {
            AuctionImages = new List<FilePathModel>();
            ABUsers = new List<ABUserModel>();
        }
    }
}