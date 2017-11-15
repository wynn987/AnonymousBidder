using System.Web.Mvc;

namespace AnonymousBidder.Controllers
{
    public class ErrorController : Controller
    {
        [AllowAnonymous]
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
