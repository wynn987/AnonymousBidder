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
using System.IO;

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
            abuserModel.Token = null;

            ABUser currentUser = GetUserByUserName(vm.EmailAddress);
            currentUser.Alias = vm.Alias;
            currentUser.Password = vm.Password;
            currentUser.Token = null;

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
            abuserModel.Money = vm.Money;

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

        internal bool DuplicateBidderEmailCheck(string emailAddress)
        {
            var user = _userRepository.FindBy(x => x.Email.Equals(emailAddress, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            return (user == null ? true : false);
        }





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
                Money = abuserModel.Money,
                ABUser_AuctionGUID = abuserModel.ABUser_AuctionGUID
            };
            _userRepository.Add(abuser);
            return abuser;
        }

       

        internal string GetSiteKey()
        {
            var parent = Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName;
            StreamReader sr = new StreamReader(parent + "\\abc.txt");
            sr.ReadLine();
            sr.ReadLine();
            return sr.ReadLine();
        }

        internal string GetSecretKey()
        {
            var parent = Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName;
            StreamReader sr = new StreamReader(parent + "\\abc.txt");
            sr.ReadLine();
            return sr.ReadLine();
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

       
        public bool checkAuctionIdExists(Guid auctionGuid)
        {
            var haveGuid = _userRepository.FindBy(x => x.ABUser_AuctionGUID == auctionGuid).FirstOrDefault();
            return haveGuid != null;
        }
        

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

        public UserRole getAdminRoleGUID() {
            var adminRole = _roleRepository.FindBy(x => x.UserRoleName == "ADMIN").FirstOrDefault();
            return adminRole;

        }

        public ABUser GetUserByUserName(string username)
        {
            return _userRepository.FindBy(x => x.Email == username).FirstOrDefault();
        }

        public ABUser GetUserByGUID(string guid)
        {
            Guid tempGuid = new Guid(guid);
            return _userRepository.FindBy(x => x.ABUserGUID == tempGuid ).FirstOrDefault();
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