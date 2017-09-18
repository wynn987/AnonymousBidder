using System.Web.Mvc;

namespace AnonymousBidder.Controllers
{
    public class ErrorController : Controller
    {
        [AllowAnonymous]
        // The 404 action handler
        public ActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;
            return View();
        }
        [AllowAnonymous]
        public ActionResult Error()
        {
            //Error page
            return View();
        }

    }
}
