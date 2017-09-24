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
        /// <summary>
        /// Controller function for system to save admin's new auction
        /// </summary>
        /// <returns>ServiceResult with new Seller Guid as Params</returns>
        [HttpPost]
        [AdminFilter]
        public ActionResult Save(AuctionCreateViewModel vm)
        {
            ServiceResult result = _auctionService.AddAuction(vm);
            if (result.Success)
            {
                return RedirectToAction("SendRegistrationEmail", result);
            }
            return RedirectToAction("FailedtoCreate", result);
        }
        /// <summary>
        /// Controller function to send registration email to seller when admin creates successfully
        /// Input ServiceReslt has seller GUID as Params
        /// </summary>
        [AdminFilter]
        public ActionResult SendRegistrationEmail(ServiceResult result)
        {
            Guid sellerGuid = Guid.Parse(result.Params);
            string registrationPath = GenerateEmailRegistrationCode(sellerGuid);
            ServiceResult emailResults = _auctionService.SendEmail(registrationPath, sellerGuid);

            return View(emailResults);
        }
        /// <summary>   
        /// Function to generate seller registration callback url
        /// </summary>
        /// <param name="sellerGuid"></param>
        /// <returns></returns>
        private string GenerateEmailRegistrationCode(Guid sellerGuid)
        {
            string code = Utilities.CreateRandomCode();
            return Url.Action("RegisterSeller", "Account", new { sellerGuid = sellerGuid, code = code }, protocol: Request.Url.Scheme);
        }
        /// <summary>
        /// Controller function to display server errors if auction not successfully created
        /// </summary>
        [AdminFilter]
        public ActionResult FailedtoCreate(ServiceResult result)
        {
            return View(result.ErrorMessage);
        }
    }
}