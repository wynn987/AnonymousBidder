using AnonymousBidder.Common;
using AnonymousBidder.Services;
using AnonymousBidder.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AnonymousBidder.Controllers
{
    public class AuctionController : Controller
    {
        private UserInfoModel UserInfoModel
        {
            get { return (UserInfoModel)System.Web.HttpContext.Current.Session["UserLoginKey"]; }
        }
        private readonly AuctionService _auctionService;
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
            return View(new AuctionCreateViewModel());
        }
    }
}