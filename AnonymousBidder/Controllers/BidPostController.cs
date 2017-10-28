﻿using AnonymousBidder.Common;
using AnonymousBidder.Services;
using AnonymousBidder.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AnonymousBidder.Controllers
{
    public class BidPostController : Controller
    {
        private readonly BidPostService _bidPostService;
        public BidPostController()
        {
            _bidPostService = new BidPostService();
        }

        private UserInfoModel UserInfoModel
        {
            get { return (UserInfoModel)System.Web.HttpContext.Current.Session["UserLoginKey"]; }
        }

        // GET: BidPost
        //[BidderFilter]
        public ActionResult BidPost()
        {
            if (UserInfoModel == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string userEmail = UserInfoModel.Email;
            BidPostViewModel model = _bidPostService.RetrieveUserGUID(userEmail);
            if (model == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(model);
        }
        
        [HttpPost]
        //[BidderFilter]
        public ActionResult BidPost(BidPostViewModel data, string returnUrl) 
        {
            decimal bid = data.BidModel.BidPlaced;
            string userEmail = UserInfoModel.Email;
            _bidPostService.updateAuctionBid(userEmail, bid);
            
            BidPostViewModel model = _bidPostService.RetrieveUserGUID(userEmail);
            if (model == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(model);
        }

        // new methods here
    }
}