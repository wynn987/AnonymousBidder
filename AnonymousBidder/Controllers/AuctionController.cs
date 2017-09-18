using AnonymousBidder.Common;
using AnonymousBidder.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AnonymousBidder.Controllers
{
    public class AuctionController : Controller
    {
        AuctionService _auctionService;
        public AuctionController()
        {
            _auctionService = new AuctionService();
        }
        [BidderFilter]
        // GET: Auction
        public ActionResult Item()
        {
            return View();
        }
        [AdminFilter]
        public ActionResult Create()
        {
            return View();
        }
    }
}