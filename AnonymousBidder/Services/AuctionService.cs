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
using System.Drawing;
using System.Net;


namespace AnonymousBidder.Services
{
    public class AuctionService
    {
        FilePathRepository _filePathRepository;
        AuctionRepository _auctionRepository;
        UserRoleRepository _userRoleRepository;
        ABUserRepository _abUserRepository;
        BidRepository _bidRepository;
        UnitOfWork _unitOfWork;

        public AuctionService()
        {
            _unitOfWork = new UnitOfWork(new AnonymousBidderDataContext());
            _filePathRepository = new FilePathRepository(_unitOfWork);
            _auctionRepository = new AuctionRepository(_unitOfWork);
            _abUserRepository = new ABUserRepository(_unitOfWork);
            _userRoleRepository = new UserRoleRepository(_unitOfWork);
            _bidRepository = new BidRepository(_unitOfWork);
        }



        
        internal ServiceResult AddAuction(AuctionCreateViewModel vm)
        {
            vm = StripStringsAuctionCreate(vm);
            ServiceResult validAuction = ValidateAuction(vm.Auction);
            ServiceResult validSeller = ValidateSeller(vm.Seller);
            if (validAuction.Success && validSeller.Success)
            {
                
               
                bool checkSellerEmailExist = DuplicateEmailCheck(vm.Seller.Email);


                if (checkSellerEmailExist)
                {
                    Auction addAuctionSuccess = SaveAuction(vm.Auction);
                    Guid addUserSuccess = SaveSeller(vm.Seller, addAuctionSuccess.AuctionGUID);


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
            }
            return new ServiceResult()
            {
                ErrorMessage = validAuction.ErrorMessage,
                Success = false
            };
        }


        internal bool DuplicateEmailCheck(string emailAddress)
        {
            var user = _abUserRepository.FindBy(x => x.Email.Equals(emailAddress, StringComparison.InvariantCultureIgnoreCase) ).FirstOrDefault();
            return (user == null ? true : false);
        }

        private AuctionCreateViewModel StripStringsAuctionCreate(AuctionCreateViewModel vm)
        {
            vm.Auction.ItemName = Utilities.RemoveSpecialCharacters(vm.Auction.ItemName);
            return vm;
        }

        internal void StoreCodetoGuid(Guid sellerGuid, string code)
        {
            var user = _abUserRepository.FindBy(x => x.ABUserGUID == sellerGuid).FirstOrDefault();

            if (user != null)
            {
                user.Token = code;
                Commit();
            }
        }

        internal Guid StoreCodetoGuid2(Guid sellerGuid)
        {
            ABUser seller = _abUserRepository.FindBy(x => x.ABUserGUID == sellerGuid).FirstOrDefault();
            Guid auctionGuid = seller.Auction.AuctionGUID;
            return auctionGuid;

            if (seller != null)
            {
                Commit();
            }

        }
        
        
        internal ServiceResult SendEmail(string registrationPath, Guid sellerGuid, string bidderRegistrationPath)
        {
            ABUser seller = _abUserRepository.FindBy(x => x.ABUserGUID == sellerGuid).FirstOrDefault();
            ABUser bidderQr = _abUserRepository.FindBy(x => x.ABUser_AuctionGUID == seller.ABUser_AuctionGUID).FirstOrDefault();

            if (seller != null
                && seller.Role != null
                && seller.Role.UserRoleName == "SELLER")
            {

                var url = string.Format("http://chart.apis.google.com/chart?cht=qr&chs={1}x{2}&chl={0}", bidderRegistrationPath, "250", "250");
                WebResponse response = default(WebResponse);
                Stream remoteStream = default(Stream);
                StreamReader readStream = default(StreamReader);
                WebRequest request = WebRequest.Create(url);
                response = request.GetResponse();
                remoteStream = response.GetResponseStream();
                readStream = new StreamReader(remoteStream);
                System.Drawing.Image img = System.Drawing.Image.FromStream(remoteStream);
                //img.Save("D:/QRCode/" + txtCode.Text + "facebook.png");
                response.Close();
                remoteStream.Close();
                readStream.Close();


                using (MemoryStream ms = new MemoryStream())
                {
                    img.Save(ms, img.RawFormat);
                    byte[] imageBytes = ms.ToArray();

                    string attachment = Convert.ToBase64String(imageBytes);

                    string body = @"<p>Your auction has been listed.</p>

                                    <p>Please kindly click <a href=" + registrationPath + @">here</a> to register and view the auction.</p>
                                   
                                    <p>Thank you,</p>
                              
                                    <p>AnonymousBidder Team</p>

                                    <p>AnonymousBidder Pte. Ltd.</p>

                                    <p><i>This is a system auto-generated email. Please do not reply to this email. </i></p>";





                    EmailHelper.SendMail("anonymousbidder3103@gmail.com", seller.Email, "Your auction has been listed", body, "", "smtp_anonymousbidder", attachment);
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

        #region Find seller auction id via seller email
        internal ABUser ViewSellerAuctionIdViaEmail(string sellerEmail)
        {
            ABUser queryResultObj = _abUserRepository.FindBy(x => x.Email == sellerEmail).FirstOrDefault();
            return queryResultObj;
        }
        #endregion

        #region View Auction Item by seller
        internal AuctionItemViewModel ViewSellerAuction(string sellerEmail)
        {
            AuctionModel postedAuctionModel = new AuctionModel();       
            BidModel bidInfoModel = new BidModel();                     
            ABUserModel bidderInfoModel = new ABUserModel();            

            
            var sellerInfoObj = _abUserRepository.FindBy(x => x.Email == sellerEmail).FirstOrDefault();

            var postedAuctionObj = _auctionRepository.FindBy(x => x.AuctionGUID == sellerInfoObj.ABUser_AuctionGUID).FirstOrDefault();

            var bidInfoObj = _bidRepository.FindBy(x => x.Bid_AuctionGUID == postedAuctionObj.AuctionGUID).FirstOrDefault();

            try
            {
                var auctionBidderObj = _abUserRepository.FindBy(x => x.ABUserGUID == bidInfoObj.Bid_ABUserGUID && x.ABUser_AuctionGUID == bidInfoObj.Bid_AuctionGUID).FirstOrDefault();

                bidderInfoModel.Alias = auctionBidderObj.Alias;

                bidInfoModel.BidPlaced = bidInfoObj.BidPlaced;
            }
            catch
            {
                bidderInfoModel.Alias = "No Bidders";
                bidInfoModel.BidPlaced = -1;
            }


            postedAuctionModel.AuctionGUID = postedAuctionObj.AuctionGUID;
            postedAuctionModel.ItemName = postedAuctionObj.ItemName;
            postedAuctionModel.StartingBid = postedAuctionObj.StartingBid;
            postedAuctionModel.StartDate = postedAuctionObj.StartDate;
            postedAuctionModel.EndDate = postedAuctionObj.EndDate;
            postedAuctionModel.AuctionOver = postedAuctionObj.AuctionOver;
            postedAuctionModel.SellerSent = postedAuctionObj.SellerSent;
            postedAuctionModel.BuyerReceived = postedAuctionObj.BuyerReceived;


            if (postedAuctionObj != null)
            {
                if (bidInfoModel != null)
                {
                    return new AuctionItemViewModel()
                    {
                        auctionItem = postedAuctionModel,
                        bidderInfo = bidderInfoModel,
                        bidInfo = bidInfoModel
                    };

                }
                else
                {
                    return new AuctionItemViewModel()
                    {
                        auctionItem = postedAuctionModel
                    };
                }
            }

            return null;
        }
        #endregion

        #region query seller by GUID
        internal Auction ViewAuctionByGUID(Guid auctionGUID)
        {
            Auction queryResultObj = _auctionRepository.FindBy(x => x.AuctionGUID == auctionGUID).FirstOrDefault();
            return queryResultObj;
        }
        #endregion

        #region Save Auction Item Seller Shipping Status
        internal ServiceResult SaveSellerShippingStatus(Auction auctionItemVM)
        {


            _auctionRepository.Update(auctionItemVM);

            bool commitSuccess = Commit();
            if (commitSuccess)
            {
                return new ServiceResult()
                {
                    Success = true
                };
            }
            return new ServiceResult();
        }
        #endregion

        #region Save
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
        
        private bool SaveFile(IEnumerable<HttpPostedFileBase> files, Guid auctionGuid)
        {
            if (files != null)
            {
                HttpPostedFileBase file = files.First();
                var fileName = Path.GetFileName(file.FileName);
                var physicalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "/App_Data/Auction_Images", fileName);

                file.SaveAs(physicalPath);

                FilePath filePath = new FilePath()
                {
                    FilePathGUID = Guid.NewGuid(),
                    FilePathName = physicalPath,
                    FilePath_AuctionGUID = auctionGuid
                };

                _filePathRepository.Add(filePath);
            }

            return true;
        }
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
                StartingBid = auctionModel.StartingBid,
                AuctionOver = false
            };
            _auctionRepository.Add(auction);
            return auction;
        }
        
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