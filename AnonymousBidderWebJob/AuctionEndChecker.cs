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
            throw new NotImplementedException();
        }

        private void SendToWinner(Auction auction)
        {
            throw new NotImplementedException();
        }

        private void SendToSeller(Auction auction)
        {
            throw new NotImplementedException();
        }

        private void Commit()
        {
            _unitOfWork.Commit();
        }
    }
}