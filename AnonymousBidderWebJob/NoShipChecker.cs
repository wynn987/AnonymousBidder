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
    internal class NoShipChecker
    {
        readonly AuctionRepository _auctionRepository;
        readonly IUnitOfWork _unitOfWork;
        public NoShipChecker()
        {
            _unitOfWork = new UnitOfWork(new AnonymousBidderDataContext());
            _auctionRepository = new AuctionRepository(_unitOfWork);
        }
        internal void RefundPayment()
        {
            DateTime localTime = Util.LocalDateTimeNow;

            //Get all unclosed auction
            List<Auction> unclosedAuctions = _auctionRepository.FindBy(x => x.AuctionOver != true 
                                                                         && x.EndDate <= localTime 
                                                                         && x.CurrentBid != null).ToList();
            foreach (Auction auction in unclosedAuctions)
            {
                // If 30 days has passed without receiving the item
                if (auction.EndDate.AddDays(30) > localTime && !auction.SellerSent)
                {
                    //Send refund email
                    string body = @"<p>The seller did not deliver the item.</p>

                                        <p>You will receive full refund and your information will be deleted.</p>

                                        <p>Thank you,</p>
                              
                                        <p>AnonymousBidder Team</p>

                                        <p>AnonymousBidder Pte. Ltd.</p>
                                
                                        <p><i>This is a system auto-generated email. Please do not reply to this email. </i></p>";

                    EmailHelper.SendMail("anonymousbidder3103@gmail.com", auction.CurrentBid.Bidder.Email, "The seller did not deliver", body, "", "smtp_anonymousbidder");
                    //Delete all related info
                    _auctionRepository.Delete(auction);
                }
            }
            Commit();
        }

        private void Commit()
        {
            _unitOfWork.Commit();
        }
    }
}