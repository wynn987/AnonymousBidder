using System;
using System.Configuration;

namespace AnonymousBidderWebJob
{
    class Program
    {
        private static int recurrencyInterval = 60000;
        public static void Main(string[] args)
        {
            AuctionEndChecker auctionEndChecker = new AuctionEndChecker();
            NoShipChecker noShipChecker = new NoShipChecker();
            while (true)
            {
                try
                {
                    auctionEndChecker.SendNotificationsandCloseAuctions();
                    noShipChecker.RefundPayment();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                System.Threading.Thread.Sleep(recurrencyInterval);
            }
        }
    }
}
