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

namespace AnonymousBidder.Services
{
    public class AuctionService
    {
        FilePathRepository _filePathRepository;
        AuctionRepository _auctionRepository;
        UnitOfWork _unitOfWork;

        public AuctionService()
        {
            _unitOfWork = new UnitOfWork(new AnonymousBidderDataContext());
            _filePathRepository = new FilePathRepository(_unitOfWork);
            _auctionRepository = new AuctionRepository(_unitOfWork);
        }

        //TODO: Complete Function
        /// <summary>
        /// Point of access from controller's Save function
        /// </summary>
        /// <param name="vm"></param>
        /// <returns>Service Result indicating pass or fail and any relevant error message</returns>
        internal ServiceResult AddAuction(AuctionCreateViewModel vm)
        {
            //Validate Data
            ServiceResult validAuction = ValidateAuction(vm.Auction);
            //Save Auction
            ServiceResult validFilePath = ValidateFilePath(vm.Files);
            //Save File
            if (validAuction.Success && validFilePath.Success)
            {
                bool addAuctionSuccess = SaveAuction(vm.Auction);
                bool addFileSuccess = SaveFile(vm.Files);
                bool commitSuccess = Commit();
            }
            throw new NotImplementedException();
        }

        //TODO: Complete Function
        /// <summary>
        /// Save image file
        /// </summary>
        /// <param name="files"></param>
        /// <returns>true if success, false if fail</returns>
        private bool SaveFile(IEnumerable<HttpPostedFileBase> files)
        {
            throw new NotImplementedException();
        }

        //TODO: Complete Function
        /// <summary>
        /// Save auction details
        /// </summary>
        /// <param name="auction"></param>
        /// <returns>true if success, false if fail</returns>
        private bool SaveAuction(AuctionModel auction)
        {
            throw new NotImplementedException();
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

        //TODO: Complete function
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
            if ()

            results.Success = results.ErrorMessage == string.Empty ? true : false;
            return results;
        }

        //TODO: Complete function
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
    }
}