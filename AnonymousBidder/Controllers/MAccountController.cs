using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using AnonymousBidder.Services;
using AnonymousBidder.Data.Entity;
using AnonymousBidder.ViewModels;
using AnonymousBidder.Common;


namespace AnonymousBidder.Controllers
{
    public class MAccountController : Controller
    {
        AccountService AccountService;
        public MAccountController()
        {
            AccountService = new AccountService();

        }

        [HttpPost]
        [AdminFilter]
        //[AllowAnonymous]
        public ActionResult RegisterModerator(MAccountCreateViewModel model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return DoRegisterModerator(model, returnUrl);
        }

        [AdminFilter]
        //[AllowAnonymous]
        public ActionResult RegisterModerator(string returnUrl)
        {
            MAccountCreateViewModel model = new MAccountCreateViewModel();
            //Request for cookie
            HttpCookie cookie = Request.Cookies["AnonymousBidder"];

            if (cookie != null)
            {
                try
                {
                    return DoRegisterModerator(model, returnUrl);
                }
                catch (Exception)
                {
                }
            }
            ViewBag.ReturnUrl = returnUrl;

            return View(model);
        }

        private ActionResult DoRegisterModerator(MAccountCreateViewModel ModeratorViewModel, string returnUrl)
        {

            var hashedPassword = Utilities.CreatePasswordHash(ModeratorViewModel.Password, ModeratorViewModel.EmailAddress);
            MAccountCreateViewModel ModViewModel = new MAccountCreateViewModel();
            ModViewModel.Password = hashedPassword;
            ModViewModel.EmailAddress = ModeratorViewModel.EmailAddress;
            ModViewModel.ConfirmPassword = hashedPassword;
            ModViewModel.Alias = ModeratorViewModel.Alias;

            ServiceResult result = new ServiceResult();
            result = AccountService.AddModeratorAccount(ModViewModel);
            if (result.Success)
            {
                return RedirectToAction("ModRegistrationSuccess", result);
            }
            return RedirectToAction("RegisterFail", result);

        }


    }


}