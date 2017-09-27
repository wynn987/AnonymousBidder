using AnonymousBidder.Common;
using AnonymousBidder.Data;
using AnonymousBidder.Data.Entity;
using AnonymousBidder.Data.Infrastructure;
using AnonymousBidder.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnonymousBidderWebJob
{
    internal class AuctionEndChecker
    {
        readonly AuctionRepository _auctionRepository;
        readonly IUnitOfWork _unitOfWork;
        public AuctionEndChecker()
        {
            _unitOfWork = new UnitOfWork(new AnonymousBidderDataContext());
            _auctionRepository = new AuctionRepository(_unitOfWork);
        }
        internal void SendNotificationsandCloseAuctions()
        {
            DateTime localTime = Util.LocalDateTimeNow;

            //Get all unclosed auction
            List<Auction> unclosedAuctions = _auctionRepository.FindBy(x => x.AuctionOver != true && x.EndDate <= localTime).ToList();
            //For each auction, close auction and send email to seller, winner and losers
            foreach(Auction auction in unclosedAuctions)
            {
                auction.AuctionOver = true;
                SendToSeller(auction);
                SendToWinner(auction);
                SendToLosers(auction);
            }
            Commit();
        }

        private void SendToLosers(Auction auction)
        {
            // If there has been at least one bid
            if (auction.Bid != null)
            {
                // Find all buyers that isnt a winner
                List<ABUser> losers = auction.ABUsers.Where(x => x.Role != null 
                                                              && x.Role.UserRoleName == "BUYER" 
                                                              && x.ABUserGUID != auction.Bid.Bid_ABUserGUID).ToList();
                string body = @"<p>The auction you have registed for has ended.</p>

                                        <p>Unfortunately, you did not place the highest bid.</p>

                                        <p>Thank you,</p>
                              
                                        <p>AnonymousBidder Team</p>

                                        <p>AnonymousBidder Pte. Ltd.</p>
                                
                                        <p><i>This is a system auto-generated email. Please do not reply to this email. </i></p>";
                foreach(ABUser loser in losers)
                {
                    EmailHelper.SendMail("anonymousbidder3103@gmail.com", loser.Email, "The auction you registered for has ended", body, "", "smtp_anonymousbidder");
                }
            }
        }
        //TODO: Ask if still need to ask winner to deposit
        private void SendToWinner(Auction auction)
        {
            // If there has been at least one bid
            if (auction.Bid != null && auction.Bid.User != null)
            {
                // Find all buyers that isnt a winner
                string body = @"<p>The auction you have registed for has ended.</p>

                                        <p>You have placed the highest bid and won the auction.</p>

                                        <p>Thank you,</p>
                              
                                        <p>AnonymousBidder Team</p>

                                        <p>AnonymousBidder Pte. Ltd.</p>
                                
                                        <p><i>This is a system auto-generated email. Please do not reply to this email. </i></p>";

            EmailHelper.SendMail("anonymousbidder3103@gmail.com", auction.Bid.User.Email, "The auction you have registered for has ended", body, "", "smtp_anonymousbidder");
            }
        }

        private void SendToSeller(Auction auction)
        {
            ABUser seller = auction.ABUsers.Where(x => x.Role != null && x.Role.UserRoleName == "SELLER").FirstOrDefault();
            if (seller != null)
            {
                // Find all buyers that isnt a winner
                string body = @"<p>The auction you have listed has ended.</p>

                                        <p>Please ship the item to the highest bidder to receive the payment</p>

                                        <p>Thank you,</p>
                              
                                        <p>AnonymousBidder Team</p>

                                        <p>AnonymousBidder Pte. Ltd.</p>
                                
                                        <p><i>This is a system auto-generated email. Please do not reply to this email. </i></p>";

                EmailHelper.SendMail("anonymousbidder3103@gmail.com", seller.Email, "The auction you have listed has ended", body, "", "smtp_anonymousbidder");
            }
        }

        private void Commit()
        {
            _unitOfWork.Commit();
        }
    }
}