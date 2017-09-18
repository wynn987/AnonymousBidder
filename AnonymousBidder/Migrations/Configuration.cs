namespace AnonymousBidder.Migrations
{
    using AnonymousBidder.Common;
    using AnonymousBidder.Data.Entity;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;

    internal sealed class Configuration : DbMigrationsConfiguration<AnonymousBidder.Data.AnonymousBidderDataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(AnonymousBidder.Data.AnonymousBidderDataContext context)
        {

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            try
            {
                //---------- ROLES --------------
                UserRole admin = new UserRole
                {
                    UserRoleGUID = Guid.NewGuid(),
                    UserRoleName = "ADMIN",
                };
                UserRole seller = new UserRole
                {
                    UserRoleGUID = Guid.NewGuid(),
                    UserRoleName = "SELLER",
                };
                UserRole bidder = new UserRole
                {
                    UserRoleGUID = Guid.NewGuid(),
                    UserRoleName = "BIDDER",
                };
                context.Role.AddOrUpdate(
                    admin, bidder, seller
                );


                //----------- AUCTION ---------------
                Auction auction = new Auction
                {
                    AuctionGUID = Guid.NewGuid(),
                    EndDate = DateTime.Now.AddDays(1),
                    StartDate = DateTime.Now,
                    ItemName = "Rare Pepe",
                    StartingBid = (decimal) 10000.00,
                    BuyerReceived = false,
                    SellerSent = false
                };
                context.Auction.AddOrUpdate(
                    auction
                );

                //----------- FILE PATH ------------
                string fileLocation = AppDomain.CurrentDomain.BaseDirectory + "/App_Data/Auction_Images/TestAuction.jpg";
                FilePath filePath = new FilePath
                {
                    FilePathGUID = Guid.NewGuid(),
                    FilePathName = fileLocation,
                    FilePath_AuctionGUID = auction.AuctionGUID
                };
                context.FilePath.AddOrUpdate(
                    filePath
                );

                //------------ USERS -------------
                ABUser bidUser = new ABUser
                {
                    Alias = "NotShadyGuy",
                    Email = "wynn987@gmail.com",
                    Password = Utilities.CreatePasswordHash("notshadypassword", "wynn987@gmail.com"),
                    ABUser_UserRoleGUID = bidder.UserRoleGUID,
                    ABUserGUID = Guid.NewGuid(),
                    ABUser_AuctionGUID = auction.AuctionGUID
                };
                ABUser sellerUser = new ABUser
                {
                    Alias = "NotShadyGuy",
                    Email = "wynn1920@gmail.com",
                    Password = Utilities.CreatePasswordHash("notshadypassword", "wynn1920@gmail.com"),
                    ABUser_UserRoleGUID = seller.UserRoleGUID,
                    ABUserGUID = Guid.NewGuid(),
                    ABUser_AuctionGUID = auction.AuctionGUID
                };
                ABUser adminUser = new ABUser
                {
                    Alias = "NotShadyAdmin",
                    Email = "4gameswynn@gmail.com",
                    Password = Utilities.CreatePasswordHash("notshadyadminpassword", "4gameswynn@gmail.com"),
                    ABUser_UserRoleGUID = admin.UserRoleGUID,
                    ABUserGUID = Guid.NewGuid()
                };
                context.User.AddOrUpdate(
                    adminUser, sellerUser, bidUser);

                //----------- BID --------------
                Bid bid = new Bid
                {
                    BidGUID = Guid.NewGuid(),
                    BidPlaced = (decimal)10000.00,
                    Bid_AuctionGUID = auction.AuctionGUID,
                    Bid_ABUserGUID = bidUser.ABUserGUID
                };
                context.Bid.AddOrUpdate(
                    bid
                );
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
            //
        }
    }
}
