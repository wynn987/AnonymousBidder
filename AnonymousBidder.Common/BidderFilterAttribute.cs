using AnonymousBidder.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AnonymousBidder.Common
{
    public class BidderFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            //If the browser session or authentication session has expired...
           
            if (ctx.Session["UserLoginKey"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { area = "", action = "Login", controller = "Account", returnUrl = ctx.Request.Url.AbsolutePath   }));
            }
            else
            {
                UserInfoModel userModel = (UserInfoModel)HttpContext.Current.Session["UserLoginKey"];
               if (userModel.isAdmin)
               {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { area = "", action = "Error", controller = "Error", returnUrl = ctx.Request.Url.AbsolutePath }));
                }
            }
            base.OnActionExecuting(filterContext);
        }
        
    }
}
