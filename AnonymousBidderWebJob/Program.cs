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
            while (true)
            {
                try
                {
                    auctionEndChecker.SendNotificationsandCloseAuctions();
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
