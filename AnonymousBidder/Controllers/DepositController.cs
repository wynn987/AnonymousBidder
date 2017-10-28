using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

using AnonymousBidder.Data;
using System.Collections.Generic;
using AnonymousBidder.Common;
using System.Linq;
using AnonymousBidder.Services;
using AnonymousBidder.Data.Entity;
using AnonymousBidder.ViewModels;



namespace AnonymousBidder.Controllers
{
    public class DepositController : Controller
    {

        AccountService AccountService;
        public DepositController()
        {
            AccountService = new AccountService();

        }


        [BidderFilter]
        [HttpPost]
        //[AllowAnonymous]
        public ActionResult DepositMoney(DepositMoneyViewModel model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return DoDeposit(model, returnUrl);
        }


        [BidderFilter]
        //[AllowAnonymous]
        public ActionResult DepositMoney(string returnUrl)
        {
            DepositMoneyViewModel model = new DepositMoneyViewModel();
            //Request for cookie
            HttpCookie cookie = Request.Cookies["AnonymousBidder"];

            if (cookie != null)
            {
                try
                {
                    return DoDeposit(model, returnUrl);
                }
                catch (Exception)
                {
                }
            }
            ViewBag.ReturnUrl = returnUrl;

            return View(model);
        }


        private ActionResult DoDeposit(DepositMoneyViewModel model, string returnUrl)
        {
            ServiceResult result = new ServiceResult();
            result = AccountService.UpdateAccountWithMoney(model);
            if (result.Success)
            {
                return RedirectToAction("DepositSuccess", result);
            }
            return RedirectToAction("DepositFail", result);
        }
    }
}