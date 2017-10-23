using AnonymousBidder.Common;
using AnonymousBidder.Data;
using AnonymousBidder.Data.Entity;
using AnonymousBidder.Data.Infrastructure;
using AnonymousBidder.Data.Repository;
using AnonymousBidder.Models;
using AnonymousBidder.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnonymousBidder.Services
{
    public class BidPostService
    {
        FilePathRepository _filePathRepository;
        AuctionRepository _auctionRepository;
        UserRoleRepository _userRoleRepository;
        ABUserRepository _abUserRepository;
        UnitOfWork _unitOfWork;

        public BidPostService()
        {
            _unitOfWork = new UnitOfWork(new AnonymousBidderDataContext());
            _filePathRepository = new FilePathRepository(_unitOfWork);
            _auctionRepository = new AuctionRepository(_unitOfWork);
            _abUserRepository = new ABUserRepository(_unitOfWork);
            _userRoleRepository = new UserRoleRepository(_unitOfWork);
        }

        internal BidPostViewModel RetrieveUserGUID(String email)
        {
            //Create a viewmodel thsat has AUctionModel data
            BidPostViewModel result = GetBidPostByEmail(email);
            if (result == null)
            {
                // if no such email
                result = null;
            }
            else
            {
                // if email matches bid post

            }


            // Return to controller
            //Controller redirect to view

            return result;
        }

        private BidPostViewModel GetBidPostByEmail(string email)
        {
            //Get database values and pass it to the view model
            ABUser user = _abUserRepository.FindBy(x => x.Email == email).FirstOrDefault();
            if (user != null)
            {
                BidPostViewModel result = new BidPostViewModel();
                var auction = user.Auction;
                var bid = auction.CurrentBid;

                FilePath f = _filePathRepository.FindBy(x => x.FilePath_AuctionGUID == user.ABUser_AuctionGUID).FirstOrDefault();
                result.ImageModel = new FilePathModel()
                {
                    FilePathName = f.FilePathName
                };

                result.AuctionModel = new AuctionModel()
                {
                    ItemName = auction.ItemName,
                    EndDate = auction.EndDate,
                    StartingBid = auction.StartingBid                    
                };
                result.BidModel = new BidModel()
                {
                    BidPlaced = bid.BidPlaced
                };

                if (auction == null || bid == null || f == null)
                {
                    // if any of the details cannot be found,
                    // bid post is either corrupted or not setup
                    return null;
                }

                return result;
            }
            else
            {
                return null;
            }
        }
    }
}