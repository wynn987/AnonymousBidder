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

        //TODO: Complete Item Function
        /// <summary>
        /// Controller function for bidders to bid
        /// </summary>
        /// <returns>Auction Item View</returns>
        [BidderFilter]
        public ActionResult Item()
        {
            return View();
        }
        //TODO: Test
        /// <summary>
        /// Controller function for admins to create auction
        /// </summary>
        /// <returns>Auction Item View</returns>
        [AdminFilter]
        public ActionResult Create()
        {
            return View(new AuctionCreateViewModel());
        }
        //TODO: Complete Save Function
        /// <summary>
        /// Controller function for system to save admin's new auction
        /// </summary>
        /// <returns>Auction Item View</returns>
        [HttpPost]
        [AdminFilter]
        public ActionResult Save(AuctionCreateViewModel vm)
        {
            ServiceResult result = _auctionService.AddAuction(vm);
            return RedirectToAction("Create");
        }
    }
}