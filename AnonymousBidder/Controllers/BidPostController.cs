using AnonymousBidder.Common;
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
        public ActionResult BidPost()
        {
            string userEmail = UserInfoModel.Email;
            BidPostViewModel model = _bidPostService.RetrieveUserGUID(userEmail);
            if (model == null)
            {
                return RedirectToAction("Login", "AccountController");
            }
            return View(model);
        }
        
        [HttpPost]
        public ActionResult Bid(BidPostViewModel model, string returnUrl) 
        {
            var variable = model;

            return null;
        }
        
        public ActionResult Bid(string returnUrl)
        {
            return null;
        }
    }
}