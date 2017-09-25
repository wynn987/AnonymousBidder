using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AnonymousBidder.Models;
using AnonymousBidder.ViewModels;
using AnonymousBidder.Common;
using AnonymousBidder.Data.Repository;
using AnonymousBidder.Data.Infrastructure;
using AnonymousBidder.Data;
using System.Web.Mvc;
using System.IO;
using AnonymousBidder.Data.Entity;
using System.Net.Mail;

namespace AnonymousBidder.Services
{
    public class AuctionService
    {
        FilePathRepository _filePathRepository;
        AuctionRepository _auctionRepository;
        UserRoleRepository _userRoleRepository;
        ABUserRepository _abUserRepository;
        UnitOfWork _unitOfWork;

        public AuctionService()
        {
            _unitOfWork = new UnitOfWork(new AnonymousBidderDataContext());
            _filePathRepository = new FilePathRepository(_unitOfWork);
            _auctionRepository = new AuctionRepository(_unitOfWork);
            _abUserRepository = new ABUserRepository(_unitOfWork);
            _userRoleRepository = new UserRoleRepository(_unitOfWork);
        }
        
        //TODO: Create Seller
        /// <summary>
        /// Point of access from controller's Save function
        /// </summary>
        /// <param name="vm"></param>
        /// <returns>
        /// Service Result indicating pass or fail and any relevant error message. 
        /// Returns Seller GUID for email registration
        /// </returns>
        internal ServiceResult AddAuction(AuctionCreateViewModel vm)
        {
            //Validate Data
            ServiceResult validAuction = ValidateAuction(vm.Auction);
            //Save Auction
            ServiceResult validFilePath = ValidateFilePath(vm.Files);
            ServiceResult validSeller = ValidateSeller(vm.Seller);
            //Save File
            if (validAuction.Success && validFilePath.Success && validSeller.Success)
            {
                Auction addAuctionSuccess = SaveAuction(vm.Auction);
                Guid addUserSuccess = SaveSeller(vm.Seller, addAuctionSuccess.AuctionGUID);
                bool addFileSuccess = SaveFile(vm.Files, addAuctionSuccess.AuctionGUID);

                bool commitSuccess = Commit();
                if (commitSuccess)
                {
                    return new ServiceResult()
                    {
                        Success = true,
                        Params = addUserSuccess.ToString()
                    };
                }
            }
            return new ServiceResult()
            {
                ErrorMessage = validAuction.ErrorMessage + validFilePath.ErrorMessage,
                Success = false
            };
        }

        /// <summary>
        /// Function to send email to seller to register and view his auction
        /// </summary>
        /// <param name="registrationPath"></param>
        /// <param name="sellerGuid"></param>
        /// <returns></returns>
        internal ServiceResult SendEmail(string registrationPath, Guid sellerGuid)
        {
            ABUser seller = _abUserRepository.FindBy(x => x.ABUserGUID == sellerGuid).FirstOrDefault();
            if (seller != null 
                && seller.Role != null 
                && seller.Role.UserRoleName == "SELLER")
            {
                string body = @"<p>Your auction has been listed.</p>

                                    <p>Please kindly click <a href=" + registrationPath + @">here</a> to register and view the auction.</p>

                                    <p>Thank you,</p>
                              
                                    <p>AnonymousBidder Team</p>

                                    <p>AnonymousBidder Pte. Ltd.</p>
                                
                                    <p><i>This is a system auto-generated email. Please do not reply to this email. </i></p>";

                EmailHelper.SendMail("anonymousbidder3103@gmail.com", seller.Email, "Your auction has been listed", body, "", "smtp_anonymousbidder");

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
        #region Save
        /// <summary>
        /// Save Seller when admin creates auction
        /// </summary>
        /// <param name="files"></param>
        /// <returns>true if success, false if fail</returns>
        private Guid SaveSeller(ABUserModel ABUserModel, Guid auctionGUID)
        {
            Guid sellerRoleGuid = _userRoleRepository.FindBy(x => x.UserRoleName == "SELLER").FirstOrDefault().UserRoleGUID;
            if (sellerRoleGuid != null && sellerRoleGuid != Guid.Empty)
            {
                ABUser abUser = new ABUser()
                {
                    ABUserGUID = Guid.NewGuid(),
                    ABUser_AuctionGUID = auctionGUID,
                    ABUser_UserRoleGUID = sellerRoleGuid,
                    Email = ABUserModel.Email,
                };
                _abUserRepository.Add(abUser);
                return abUser.ABUserGUID;
            }
            return Guid.Empty;
        }

        /// <summary>
        /// Save image file when admin creates auction
        /// </summary>
        /// <param name="files"></param>
        /// <returns>true if success, false if fail</returns>
        private bool SaveFile(IEnumerable<HttpPostedFileBase> files, Guid auctionGuid)
        {
            // The Name of the Upload component is "files"
            if (files != null)
            {
                HttpPostedFileBase file = files.First();
                // Some browsers send file names with full path. This needs to be stripped.
                var fileName = Path.GetFileName(file.FileName);
                var physicalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "/App_Data/Auction_Images", fileName);

                // The files are not actually saved in this demo
                file.SaveAs(physicalPath);

                FilePath filePath = new FilePath(){
                    FilePathGUID = Guid.NewGuid(),
                    FilePathName = physicalPath,
                    FilePath_AuctionGUID = auctionGuid
                };

                _filePathRepository.Add(filePath);
            }

            // Return an empty string to signify success
            return true;
        }

        //TODO: Complete Function
        /// <summary>
        /// Save auction details when admin creates auction
        /// </summary>
        /// <param name="auction"></param>
        /// <returns>Returns newly created auction</returns>
        private Auction SaveAuction(AuctionModel auctionModel)
        {
            Auction auction = new Auction()
            {
                AuctionGUID = Guid.NewGuid(),
                BuyerReceived = false,
                EndDate = auctionModel.EndDate,
                ItemName = auctionModel.ItemName,
                SellerSent = false,
                StartDate = auctionModel.StartDate,
                StartingBid = auctionModel.StartingBid
            };
            _auctionRepository.Add(auction);
            return auction;
        }

        //TODO: Add logging function for exceptions
        /// <summary>
        /// This function will "officially" save all edits to the db since this function was last called
        /// </summary>
        /// <returns>Auction Item View</returns>
        private bool Commit()
        {
            try
            {
                _unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                return false;
            }
        }
        #endregion
        #region Validate
        /// <summary>
        /// Validate if file path is in valid format before saving
        /// </summary>
        /// <param name="files"></param>
        /// <returns>return true if success, false if fail</returns>
        private ServiceResult ValidateFilePath(IEnumerable<HttpPostedFileBase> files)
        {
            ServiceResult results = new ServiceResult
            {
                ErrorMessage = string.Empty
            };

            if (files.Count() > 1)
            {
                results.ErrorMessage = "Select one file only\n";
            }
            if (files.First().FileName == string.Empty)
            {
                results.ErrorMessage = "No file detected\n";
            }
            if (files.First().ContentType.ToLower() != "image/jpeg" 
                && files.First().ContentType.ToLower() != "image/png")
            {
                results.ErrorMessage = "File is not an image\n";
            }

            results.Success = (results.ErrorMessage == string.Empty || files.First() == null) ? true : false;
            return results;
        }
        
        /// <summary>
        /// Validate if auction is in valid format before saving
        /// </summary>
        /// <param name="files"></param>
        /// <returns>return true if success, false if fail</returns>
        private ServiceResult ValidateAuction(AuctionModel auction)
        {
            ServiceResult results = new ServiceResult
            {
                ErrorMessage = string.Empty
            };

            if (auction.ItemName == string.Empty || auction.ItemName == null)
            {
                results.ErrorMessage += "Item name cannot be empty\n";
            }
            if (auction.StartDate == null)
            {
                results.ErrorMessage += "Start date cannot be empty\n";
            }
            if (auction.EndDate == null)
            {
                results.ErrorMessage += "End date cannot be empty\n";
            }
            if (auction.StartingBid < 0 || auction.StartingBid >= 9999999999)
            {
                results.ErrorMessage += "Starting bid must be within 1 to 9999999999\n";
            }
            if (auction.StartDate.Date < DateTime.Now.Date)
            {
                results.ErrorMessage += "Start date must be after today\n";
            }
            if (auction.StartDate > auction.EndDate)
            {
                results.ErrorMessage += "End date must be after start date";
            }
            results.Success = results.ErrorMessage == string.Empty ? true : false;
            return results;
        }

        /// <summary>
        /// Check if seller's email is valid when admin creating auction
        /// </summary>
        /// <param name="ABUser"></param>
        /// <returns></returns>
        private ServiceResult ValidateSeller(ABUserModel abUserModel)
        {
            ServiceResult results = new ServiceResult
            {
                ErrorMessage = string.Empty
            };
            if (abUserModel == null || abUserModel.Email == string.Empty || abUserModel.Email == null)
            {
                results.ErrorMessage = "Email is not a valid email\n";
            }
            try
            {
                MailAddress validEmail = new MailAddress(abUserModel.Email);
            }
            catch (FormatException)
            {
                results.ErrorMessage = "Invalid email formats\n";
            }

            results.Success = results.ErrorMessage == string.Empty ? true : false;
            return results;
        }
        #endregion
    }
}