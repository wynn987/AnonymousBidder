﻿using AnonymousBidder.Common;
using AnonymousBidder.Data.Entity;
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
        
        [SellerFilter]
        public ActionResult Item()
        {
            AuctionItemViewModel result = _auctionService.ViewSellerAuction(UserInfoModel.Email);
            return View(result);
        }

        [SellerFilter]
        [HttpPost]
        public ActionResult SaveSellerItemStatus(AuctionItemViewModel itemViewModel, FormCollection form)
        {
            string valueOf = form["auctionItem.SellerSent"].ToString();

            ABUser seller = _auctionService.ViewSellerAuctionIdViaEmail(UserInfoModel.Email);
            

            if (valueOf.Equals("1"))
            {
                itemViewModel.auctionItem.SellerSent = true;
            }
            else if (valueOf.Equals("0"))
            {
                itemViewModel.auctionItem.SellerSent = false;
            }
            else
            {
            }


            Auction queryObj;
            try
            {
                queryObj = _auctionService.ViewAuctionByGUID(seller.ABUser_AuctionGUID.Value);
                queryObj.SellerSent = itemViewModel.auctionItem.SellerSent;
            }
            catch
            {
                return null;
            }

            ServiceResult result = _auctionService.SaveSellerShippingStatus(queryObj);
            if (result.Success)
            {
                return RedirectToAction("Item", result);
            }
            return null;

        }
        
        [AdminFilter]
        public ActionResult Create()
        {
            return View(new AuctionCreateViewModel());
        }

        [HttpPost]
        public JsonResult CheckEmail(string emailAddress)
        {
            bool valid = _auctionService.DuplicateEmailCheck(emailAddress);
            return Json(valid, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        [AdminFilter]
        public ActionResult Save(AuctionCreateViewModel vm)
        {
            ServiceResult result = _auctionService.AddAuction(vm);

            if (result.Success)
            {
                return RedirectToAction("SendRegistrationEmail", result);

            }
            return RedirectToAction("Create");
        }
        [AdminFilter]
        public ActionResult SendRegistrationEmail(ServiceResult result)
        {
            Guid sellerGuid = Guid.Parse(result.Params);
            //Guid auctionGuid = Guid.Parse(result.Params);

            string registrationPath = GenerateEmailRegistrationCode(sellerGuid);
            string bidderRegistrationPath = GenerateEmailRegistrationCode2(sellerGuid);

            ServiceResult emailResults = _auctionService.SendEmail(registrationPath, sellerGuid, bidderRegistrationPath);

            return RedirectToAction("Create");
        }
        private string GenerateEmailRegistrationCode(Guid sellerGuid)
        {
            string code = Utilities.CreateRandomCode();
            _auctionService.StoreCodetoGuid(sellerGuid, code);
            return Url.Action("RegisterSeller", "Account", new { sellerGuid = sellerGuid, code = code }, protocol: Request.Url.Scheme);
        }

        private string GenerateEmailRegistrationCode2(Guid sellerGuid)
        {
            string code2 = Utilities.CreateRandomCode();
            Guid bidderGuid = _auctionService.StoreCodetoGuid2(sellerGuid);

            return Url.Action("RegisterBidder", "Account", new { auctionGuid = bidderGuid }, protocol: Request.Url.Scheme);
        }
        [AdminFilter]
        public ActionResult FailedtoCreate(ServiceResult result)
        {
            return View(result.ErrorMessage);
        }
    }
}