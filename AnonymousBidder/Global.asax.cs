using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AnonymousBidder
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
	        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Local
            };

            ModelBinders.Binders.DefaultBinder = new AnonymousBidder.Common.Extensions.EnumFlagsModelBinder();
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            HttpException httpException = exception as HttpException;
            //ToDO :  Log this exception in here
        }
        protected void Application_EndRequest()
        {
           
        }
    }
}
