using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AnonymousBidder.Common;
using System;

namespace AnonymousBidder
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CustomErrorHandler());
        }
        
    }
    public class CustomErrorHandler : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            Exception e = filterContext.Exception;
            filterContext.ExceptionHandled = true;
            var result = new ViewResult()
            {
                ViewName = "Error"
            }; ;
            result.ViewBag.Error = "Error Occur While Processing Your Request Please Check After Some Time";
            filterContext.Result = result;
        }
    }
}
