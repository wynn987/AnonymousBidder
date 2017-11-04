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
using System.Net;
using Newtonsoft.Json.Linq;

namespace AnonymousBidder.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        AccountService AccountService;
        public AccountController()
        {
            AccountService = new AccountService();

        }

        // Write Controller function to receive URL
        // Write View Model to tell the system what data to expect
        // Write View to receive the data
        // Write Service layer to get the data

        #region Login

        /// <summary>
        /// Check If User email address is Existing
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public JsonResult EmailExists(string emailAddress)
        {
            bool isUserExisted = false;
            ABUser user = AccountService.GetUserByUserName(emailAddress);
            isUserExisted = user != null;

            return Json(isUserExisted, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Check If User email address is Existing
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public JsonResult CheckPassword(string emailAddress, string password)
        {
            bool isValid = true;

            if (!string.IsNullOrEmpty(emailAddress) && !string.IsNullOrEmpty(password))
            {
                string hashedPassword = Utilities.CreatePasswordHash(password, emailAddress);

                isValid = AccountService.CheckUsernameAndPassword(emailAddress, hashedPassword);
            }

            return Json(isValid, JsonRequestBehavior.AllowGet);
        }

        

        
        [HttpPost]
        [AllowAnonymous]
        public ActionResult RegisterBidder(BAccountCreateViewModel model,string returnUrl)
        {
            Guid auctionGUID;
            ViewBag.ReturnUrl = returnUrl;
            auctionGUID = model.auctionGUID;

            return DoRegisterBidder(model,auctionGUID ,returnUrl);
            //return RedirectToAction("", "");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult RegisterBidder(string returnUrl)
        {
            var auctionGuid = Request.QueryString["auctionGuid"];
            BAccountCreateViewModel model = new BAccountCreateViewModel();
            model.auctionGUID = new Guid(auctionGuid);

            HttpCookie cookie = Request.Cookies["AnonymousBidder"];
            if (cookie != null)
            {
                try
                {
                    
                    return DoRegisterBidder(model, model.auctionGUID,returnUrl);
                    //return RedirectToAction("","");
                }
                catch (Exception)
                {
                }
            }
            ViewBag.ReturnUrl = returnUrl;

            return View(model);

        }
        


        [HttpPost]
        [AllowAnonymous]
        public ActionResult RegisterSeller(AccountCreateViewModel model, string returnUrl)
        {
            string code;
            Guid sellerGuid;
            ViewBag.ReturnUrl = returnUrl;
            sellerGuid = model.userGUID;
            code = model.userToken;

            return DoRegister(model, sellerGuid, code, returnUrl);
        }
        
        [HttpGet]
        [AllowAnonymous]
        public ActionResult RegisterSeller(string returnUrl)
        {
            //fetch the url sellerGUID and token code
            var sellerGuid = Request.QueryString["sellerGuid"];
            var code = Request.QueryString["code"];

            ABUser currentUser = AccountService.GetUserByGUID(sellerGuid);
            var currentUserEmail = currentUser.Email;
            ViewBag.UserEmail = currentUserEmail;

            AccountCreateViewModel model = new AccountCreateViewModel();
            model.userGUID = new Guid(sellerGuid);
            model.userToken = code;

            HttpCookie cookie = Request.Cookies["AnonymousBidder"];
            if (cookie != null)
            {
                try
                {
                    return DoRegister(model, model.userGUID, model.userToken, returnUrl);
                }
                catch (Exception)
                {
                }
            }
            ViewBag.ReturnUrl = returnUrl;

            return View(model);
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

       






        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            //var response = Request["g-recaptcha-response"];
            //string secretKey = "6LfACTcUAAAAAAUtHbQSZwH17ksmX9x2n8HRv5CB";
            //var client = new WebClient();
            //var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
            //var obj = JObject.Parse(result);
            //var status = (bool)obj.SelectToken("success");
            //ViewBag.Message = status ? "Google reCaptcha validation success" : "Google reCaptcha validation failed";
            //if (!status)
            //{
            //    throw new Exception();
            //}
            return DoLogin(model, returnUrl);
        }
        

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            LoginViewModel model = new LoginViewModel();
            //Request for cookie
            HttpCookie cookie = Request.Cookies["AnonymousBidder"];

            if (cookie != null)
            {
                try
                {
                    //some times no cookie in browser
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);

                    //Get login info from cookie
                    model.EmailAddress = ticket.Name;
                    model.HashedPassword = ticket.UserData;
                    model.RememberMe = ticket.IsPersistent;

                    return DoLogin(model, returnUrl);
                }
                catch (Exception)
                {
                }
            }
            ViewBag.ReturnUrl = returnUrl;

            return View(model);
        }

        private ActionResult DoLogin(LoginViewModel model, string returnUrl)
        {
            if (string.IsNullOrEmpty(model.HashedPassword))
                model.HashedPassword = Utilities.CreatePasswordHash(model.Password, model.EmailAddress);
            ABUser user = AccountService.GetUserByUserNameAndPassword(model.EmailAddress, model.HashedPassword);

            if (user != null) 
            {
                UserInfoModel userInfo = new UserInfoModel
                {
                    Email = user.Email,
                    Role = user.Role.UserRoleName
                };

                Session["User"] = userInfo;
                HttpSession.SetInSession(userInfo);

                FormsAuthentication.SetAuthCookie(model.EmailAddress, model.RememberMe);

                #region Remember Me
                if (model.RememberMe)
                {
                    var userData = model.HashedPassword;
                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                                1,
                                model.EmailAddress,
                                DateTime.Now,
                                DateTime.Now.AddDays(7),//Remember for 7 days
                                model.RememberMe,
                                userData);

                    string encTicket = FormsAuthentication.Encrypt(authTicket);
                    HttpCookie cookie = new HttpCookie("AnonymousBidder", encTicket);
                    cookie.Expires = authTicket.Expiration; //must do it for cookie expiration 
                    Response.Cookies.Add(cookie);
                }
                #endregion Remember Me

                if (Url.IsLocalUrl(returnUrl) 
                    && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                    && !returnUrl.StartsWith("//") 
                    && !returnUrl.StartsWith("/\\"))
                {
                    return Redirect(returnUrl);
                }

                if (userInfo.Role == "ADMIN")
                {
                    return RedirectToAction("Create", "Auction");
                }
                else if (userInfo.Role == "SELLER")
                {
                    return RedirectToAction("Item", "Auction");

                } 

                
                else if (userInfo.Role == "BIDDER")
                {
                    return RedirectToAction("BidPost", "BidPost");
                }

            }
            return View();
        }

        // register a seller
        private ActionResult DoRegister(AccountCreateViewModel model, Guid sellerGuid, string token, string returnUrl)
        {
            
            ABUser currentUser = AccountService.GetUserByUserName(model.EmailAddress);
            var currentUserGuid = currentUser.ABUserGUID;
            var currentUserToken = currentUser.Token;
            Guid tempCurrentUserGuid = sellerGuid;

            if(tempCurrentUserGuid == currentUserGuid && currentUserToken == token)
            {
                var hashedPassword = Utilities.CreatePasswordHash(model.Password, model.EmailAddress);
                AccountCreateViewModel vm = new AccountCreateViewModel();
                vm.Password = hashedPassword;
                vm.EmailAddress = model.EmailAddress;
                vm.ConfirmPassword = hashedPassword;
                vm.Alias = model.Alias;

                ServiceResult result = new ServiceResult();
                result = AccountService.AddAccount(vm);
                if (result.Success)
                {
                    return RedirectToAction("RegisterSuccess", result);
                }
                return RedirectToAction("RegisterFail", result);

            }

            return null;
        }

       

        private ActionResult DoRegisterBidder(BAccountCreateViewModel model, Guid auctionGuid, string returnUrl)
        {

            Guid tempCurrentAuctionGuid = auctionGuid;
            var hashedPassword = Utilities.CreatePasswordHash(model.Password, model.EmailAddress);
            BAccountCreateViewModel vm = new BAccountCreateViewModel();
            vm.Password = hashedPassword;
            vm.EmailAddress = model.EmailAddress;
            vm.ConfirmPassword = hashedPassword;
            vm.Alias = model.Alias;


            ServiceResult result = new ServiceResult();
                result = AccountService.AddBidderAccount(vm,tempCurrentAuctionGuid);
                if (result.Success)
                {
                    return RedirectToAction("RegisterSuccess", result);
                }
                return RedirectToAction("RegisterFail", result);

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
        
        [AllowAnonymous]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            //Request for cookie
            Response.Cookies["AnonymousBidder"].Expires = DateTime.Now.AddDays(-1);

            Session.Abandon();
            return RedirectToAction("Login");
        }
        
        [AllowAnonymous]
        public ActionResult ChangePassword()
        {
            UserInfoModel user = HttpSession.GetFromSession<UserInfoModel>();
            if (user == null) return RedirectToAction("Login");

            return View();
        }

        [AllowAnonymous]
        public ActionResult RegisterSuccess()
        {
            return View();
        }


        [AllowAnonymous]
        public ActionResult RegisterFail()
        {
            return View();
        }


        // POST: /Account/DoChangePassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult DoChangePassword(ChangePasswordViewModel model)
        {
            UserInfoModel user = HttpSession.GetFromSession<UserInfoModel>();
            if (user == null) return RedirectToAction("Login");

            if (!ModelState.IsValid)
            {
                return View("ChangePassword", model);
            }
            string hashedPassword = Utilities.CreatePasswordHash(model.OldPassword, user.Email);
            var isValidOldPassword = AccountService.DoLogin(user.Email, hashedPassword);

            if (isValidOldPassword)
            {
                ABUser ca_user = AccountService.GetUserByUserNameAndPassword(user.Email, hashedPassword);

                hashedPassword = Utilities.CreatePasswordHash(model.Password, user.Email);
                ca_user.Password = hashedPassword;
                var result = AccountService.UpdateUser(ca_user);

                if (result)
                {
                    return RedirectToAction("Login");
                }
            }
            else
            {
                ModelState.AddModelError("OldPassword", "The Old Password is not correct.");
            }
            return View("ChangePassword", model);
        }
        #endregion


    }
    public static class HttpSession
    {
        const string UserLoginKey = "UserLoginKey";

        public static void SetInSession(object value)
        {
            HttpContext.Current.Session[UserLoginKey] = value;
        }

        public static T GetFromSession<T>()
        {
            return (T)HttpContext.Current.Session[UserLoginKey];
        }
    }
}