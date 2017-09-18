using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AnonymousBidder.Controllers
{
    public class AuctionController : Controller
    {
        // GET: Auction
        public ActionResult Item()
        {
            return View();
        }
    }
}