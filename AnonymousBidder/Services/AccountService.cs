using AnonymousBidder.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AnonymousBidder.Data.Repository;
using AnonymousBidder.Data.Entity;
using AnonymousBidder.ViewModels;
using AnonymousBidder.Common;
using AnonymousBidder.Models;

namespace AnonymousBidder.Services
{
    public class AccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ABUserRepository _userRepository;
        private readonly UserRoleRepository _roleRepository;

        public AccountService()
        {
            _unitOfWork = new UnitOfWork();
            _userRepository = new ABUserRepository(_unitOfWork);
            _roleRepository = new UserRoleRepository(_unitOfWork);

        }
        /*
        *This function is for updatig user or equal to add a new seller into system
        */
        internal ServiceResult AddAccount(AccountCreateViewModel vm)
        {
            ABUserModel abuserModel = new ABUserModel();
            abuserModel.Alias = vm.Alias;
            abuserModel.Email = vm.EmailAddress;
            abuserModel.Password = vm.Password;

            ABUser currentUser = GetUserByUserName(vm.EmailAddress);
            currentUser.Alias = vm.Alias;
            currentUser.Password = vm.Password;

            _userRepository.Update(currentUser);


            bool commitSuccess = UpdateUser(currentUser);

            if (commitSuccess)
            {
                return new ServiceResult()
                {
                    Success = true,
                    Params = currentUser.ToString()
                };
            }

            return new ServiceResult()
            {
                ErrorMessage = "Error message",
                Success = false
            };
        }

        
        internal ServiceResult AddBidderAccount(BAccountCreateViewModel vm,Guid auctionGuid)
        {
            ABUserModel abuserModel = new ABUserModel();
            abuserModel.Alias = vm.Alias;
            abuserModel.Email = vm.EmailAddress;
            abuserModel.Password = vm.Password;
            abuserModel.ABUserGUID = Guid.NewGuid();
            abuserModel.ABUser_AuctionGUID = auctionGuid;
            ABUser addBidderSuccess = SaveBidderAccount(abuserModel);
            bool commitSuccess = UpdateUser(addBidderSuccess);

            if (commitSuccess)
            {
                return new ServiceResult()
                {
                    Success = true,
                    Params = addBidderSuccess.ToString()
                };
            }

            return new ServiceResult()
            {
                ErrorMessage = "Error message",
                Success = false
            };

        }
        
        

        internal ServiceResult UpdateAccountWithMoney(DepositMoneyViewModel vm)
        {
            UserInfoModel sessionVar = (UserInfoModel)HttpContext.Current.Session["User"];
            var currEmail = sessionVar.Email;
            ABUser currentUser = GetUserByUserName(currEmail);
            var currentUserMoneyBalance = currentUser.Money;
            currentUser.Money = currentUserMoneyBalance + vm.Money;
            _userRepository.Update(currentUser);


            bool commitSuccess = UpdateUser(currentUser);

            if (commitSuccess)
            {
                return new ServiceResult()
                {
                    Success = true,
                    Params = currentUser.ToString()
                };
            }

            return new ServiceResult()
            {
                ErrorMessage = "Error message",
                Success = false
            };

        }


  


        //save account for seller
        private ABUser SaveAccount(ABUserModel abuserModel)
        {
            var role = getGUID();
            ABUser abuser = new ABUser()
            {
                ABUserGUID = Guid.NewGuid(),
                Alias = abuserModel.Alias,
                Email = abuserModel.Email,
                Password = abuserModel.Password,
                Role = role
        };
            _userRepository.Add(abuser);
            return abuser;
        }

        private ABUser SaveBidderAccount(ABUserModel abuserModel)
        {
            var role = getBidderRoleGUID();
            ABUser abuser = new ABUser()
            {
                ABUserGUID = Guid.NewGuid(),
                Alias = abuserModel.Alias,
                Email = abuserModel.Email,
                Password = abuserModel.Password,
                Role = role,
                ABUser_AuctionGUID = abuserModel.ABUser_AuctionGUID
            };
            _userRepository.Add(abuser);
            return abuser;
        }

        #region Login
        public bool DoLogin(string username, string password)
        {
            var user = _userRepository.FindBy(x => x.Email == username && x.Password == password).FirstOrDefault();
            
            return user != null;
        }

        public bool CheckUsernameAndPassword(string username, string password)
        {
            var user = _userRepository.FindBy(x => x.Email == username && x.Password == password).FirstOrDefault();
            return user != null;
        }

        // get roleGUID for seller
        public UserRole getGUID()
        {
            var sellerRole = _roleRepository.FindBy(x => x.UserRoleName == "SELLER").FirstOrDefault();
            return sellerRole;

        }

        public UserRole getBidderRoleGUID()
        {
            var bidderRole = _roleRepository.FindBy(x => x.UserRoleName == "BIDDER").FirstOrDefault();
            return bidderRole;
        }

        public ABUser GetUserByUserName(string username)
        {
            return _userRepository.FindBy(x => x.Email == username).FirstOrDefault();
        }

        public ABUser GetUserByUserNameAndPassword(string username, string hashedPassword)
        {
            var user = _userRepository.FindBy(x => x.Email == username && x.Password == hashedPassword).FirstOrDefault();
            
            return user;
        }
        #endregion
        
        #region User Service
        public bool UpdateUser(ABUser user)
        {
            try
            {
                _unitOfWork.Commit();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

    }
}