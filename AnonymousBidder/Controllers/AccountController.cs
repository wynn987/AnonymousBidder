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
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

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
            }
            return View();
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

        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {

            if (ModelState.IsValid)
            {
                ABUser user = AccountService.GetUserByUserName(model.Email);

                if (user == null)
                {
                    ViewBag.NotExistingUser = "The Email Address does not exist.";
                    // Don't reveal that the user does not exist or is not confirmed
                    return View();
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link

                string code = Utilities.CreateRandomCode();
                user.Token = code;
                AccountService.UpdateUser(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { user.ABUserGUID, code = code }, protocol: Request.Url.Scheme);
                string body = @"<p>Hi " + user.Alias + @",</p>

                                <p>We received a request to reset your password for your AnonymousBidder account " + user.Email + @".</p>
                                
                                <p>Please kindly click <a href=" + callbackUrl + @">here</a> to set a new password.</p>

                                <p>If you didn't ask to change your password, please kindly ignore this email.</p>

                                <p>Your password is still safe and you can continue logging in with your current password.</p>

                                <p>Thank you,</p>
                              
                                <p>AnonymousBidder Team</p>

                                <p>AnonymousBidder Pte. Ltd.</p>
                                
                                <p><i>This is a system auto-generated email. Please do not reply to this email. </i></p>";

                EmailHelper.SendMail("anonymousbidder3103@gmail.com", model.Email, "Reset Your AnonymousBidder Password", body, "", "smtp_anonymousbidder");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = AccountService.GetUserByUserName(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            string hashedPassword = Utilities.CreatePasswordHash(model.Password, model.Email);
            user.Password = hashedPassword;
            var result = AccountService.UpdateUser(user);
            if (result)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            return View();
        }
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ChangePassword()
        {
            UserInfoModel user = HttpSession.GetFromSession<UserInfoModel>();
            if (user == null) return RedirectToAction("Login");

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
                    return RedirectToAction("Index", "Auction");
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