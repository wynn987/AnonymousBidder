using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AnonymousBidder.Common;

namespace AnonymousBidder
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleAndLogErrorAttribute());
        }
        
    }
    public class HandleAndLogErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
        }
    }
}
