using Microsoft.VisualStudio.TestTools.UnitTesting;
using AnonymousBidder.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;


namespace AnonymousBidder.Controllers.Tests
{
    [TestClass()]
    public class AuctionControllerTests
    {
        [TestMethod]
        public void AuctionItemViewTest()
        {
            var controller = new AuctionController();
            var result = controller.Item() as ViewResult;
            Assert.AreEqual("Item", result.ViewName);
        }
    }
}