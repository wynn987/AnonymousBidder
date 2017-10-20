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

        public AccountService()
        {
            _unitOfWork = new UnitOfWork();
            _userRepository = new ABUserRepository(_unitOfWork);
        }

        internal ServiceResult AddAccount(AccountCreateViewModel vm)
        {
            ABUserModel abuserModel = new ABUserModel();
            abuserModel.Alias = vm.Alias;
            abuserModel.Email = vm.EmailAddress;
            abuserModel.Password = vm.Password;
            abuserModel.ABUserGUID = Guid.NewGuid();
            abuserModel.ABUser_UserRoleGUID = Guid.Parse("16d4a3fc-8351-4620-9eee-a47302de60b0");
            ABUser addUserSuccess = SaveAccount(abuserModel);
            bool commitSuccess = UpdateUser(addUserSuccess);

            if (commitSuccess)
            {
                return new ServiceResult()
                {
                    Success = true,
                    Params = addUserSuccess.ToString()
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


  



        private ABUser SaveAccount(ABUserModel abuserModel)
        {
            ABUser abuser = new ABUser()
            {
                ABUserGUID = Guid.NewGuid(),
                Alias = abuserModel.Alias,
                Email = abuserModel.Email,
                Password = abuserModel.Password,
                ABUser_UserRoleGUID = Guid.Parse("16d4a3fc-8351-4620-9eee-a47302de60b0")
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