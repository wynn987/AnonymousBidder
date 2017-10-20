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
        public ActionResult Index()
        {
            string userEmail = UserInfoModel.Email;
            BidPostViewModel model = _bidPostService.RetrieveUserGUID(userEmail);
            return View(model);
        }


    }
}