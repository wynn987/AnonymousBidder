using AnonymousBidder.Common;
using AnonymousBidder.Data;
using AnonymousBidder.Data.Entity;
using AnonymousBidder.Data.Infrastructure;
using AnonymousBidder.Data.Repository;
using AnonymousBidder.Models;
using AnonymousBidder.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
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
        BidRepository _bidRepository;

        UnitOfWork _unitOfWork;

        public BidPostService()
        {
            _unitOfWork = new UnitOfWork(new AnonymousBidderDataContext());
            _filePathRepository = new FilePathRepository(_unitOfWork);
            _auctionRepository = new AuctionRepository(_unitOfWork);
            _abUserRepository = new ABUserRepository(_unitOfWork);
            _userRoleRepository = new UserRoleRepository(_unitOfWork);
            _bidRepository = new BidRepository(_unitOfWork);
        }

        internal BidPostViewModel RetrieveUserGUID(String email)
        {
            //Create a viewmodel thsat has AUctionModel data
            BidPostViewModel result = GetBidPostByEmail(email);
            if (result != null)
            {
                // if no such email
                return result;
            }

            return null;
        }

        private BidPostViewModel GetBidPostByEmail(string email)
        {
            //Get database values and pass it to the view model
            ABUser user = _abUserRepository.FindBy(x => x.Email == email).FirstOrDefault();
            if (user != null && user.Role.UserRoleName == "BIDDER"
                    && user.ABUser_AuctionGUID != null)
            {
                BidPostViewModel result = new BidPostViewModel();
                var auction = user.Auction;
                result.AuctionModel = new AuctionModel()
                {
                    ItemName = auction.ItemName,
                    EndDate = auction.EndDate,
                    StartingBid = auction.StartingBid
                };

                FilePath f = _filePathRepository.FindBy(x => x.FilePath_AuctionGUID == user.ABUser_AuctionGUID).FirstOrDefault();
                result.ImageModel = new FilePathModel()
                {
                    FilePathName = f.FilePathName
                };

                Auction auctionBid = _auctionRepository.FindBy(x => x.AuctionGUID == user.ABUser_AuctionGUID).FirstOrDefault();
                if (auctionBid.Auction_BidGUID == null)
                {
                    // no one has bid before
                    result.BidModel = new BidModel()
                    {
                        BidPlaced = auctionBid.StartingBid
                    };
                }
                else
                {
                    result.BidModel = new BidModel()
                    {
                        BidPlaced = auctionBid.CurrentBid.BidPlaced
                    };
                }
                
                if (auction == null || auctionBid == null || f == null)
                {
                    // if any of the details cannot be found,
                    // bid post is either corrupted or not setup
                    return null;
                }

                return result;
            }
            else
            {
                // if user cannot be found
                return null;
            }
        }

        internal ServiceResult updateAuctionBid(string email, decimal bid)
        {
            ABUser user = _abUserRepository.FindBy(x => x.Email.ToString() == email).FirstOrDefault();
            if (user != null && user.Role.UserRoleName.ToString() == "BIDDER")
            {
                Auction auctionResult = _auctionRepository.FindBy(x => x.AuctionGUID == user.ABUser_AuctionGUID).FirstOrDefault();
                if (auctionResult.Auction_BidGUID == null)
                {
                    // no one has bid before
                    if (bid > auctionResult.StartingBid)
                    {
                        // create new bid
                        Bid b = createNewBid(user, auctionResult, bid);
                        _bidRepository.Add(b);
                        // update bid here
                        auctionResult.Auction_BidGUID = b.BidGUID;
                        _unitOfWork.Commit();
                        return new ServiceResult()
                        {
                            Success = true
                        };
                    }
                }
                else
                {
                    if (bid > auctionResult.CurrentBid.BidPlaced)
                    {
                        // create new bid
                        Bid b = createNewBid(user, auctionResult, bid);
                        _bidRepository.Add(b);
                        // update bid here
                        auctionResult.Auction_BidGUID = b.BidGUID;
                        _unitOfWork.Commit();
                        return new ServiceResult()
                        {
                            Success = true
                        };
                    }
                }
            }

            return new ServiceResult()
            {
                ErrorMessage = "New bid could not be submitted.",
                Success = false
            };
        }

        private Bid createNewBid(ABUser user, Auction data, decimal bid)
        {
            Bid b = new Bid();
            b.Bid_AuctionGUID = data.AuctionGUID;
            b.Bid_ABUserGUID = user.ABUserGUID;
            b.BidGUID = Guid.NewGuid();
            b.BidPlaced = bid;
            return b;
        }

        internal ServiceResult SendEmail(ABUser user, Auction data, decimal bid)
        {
            if (user != null
                && user.Role != null
                && user.Role.UserRoleName == "BIDDER"
                && user.ABUser_AuctionGUID != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    //string htmlBody = "<img src='data:image/png;base64," + Convert.ToBase64String(imageBytes) + @"'/>";                                

                    string body = @"<p>Dear User,</p>
                                <br/>
                                <p>You have been outbidded!</p>
                                <p>Do login to submit a new bid for the auction</p>
                                <br/>   
                                <p>Thank you,</p>
                                <p>AnonymousBidder Team</p>
                                <p>AnonymousBidder Pte. Ltd.</p>
                                <br/>
                                <p><i>This is a system auto-generated email. Please do not reply to this email.</i></p>";
                    
                    EmailHelper.SendMail("anonymousbidder3103@gmail.com", user.Email, "Your auction has been listed", body, "", "smtp_anonymousbidder");
                }
                return new ServiceResult()
                {
                    Success = true
                };
            }
            return new ServiceResult()
            {
                ErrorMessage = "Could not find user",
                Success = false
            };

        }
    }

    
}