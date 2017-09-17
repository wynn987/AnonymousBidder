using AnonymousBidder.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AnonymousBidder.Data.Repository;
using AnonymousBidder.Data.Entity;

namespace AnonymousBidder.Services
{
    public class AccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserRepository _userRepository;

        public AccountService()
        {
            _unitOfWork = new UnitOfWork();
            _userRepository = new UserRepository(_unitOfWork);
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

        public User GetUserByUserName(string username)
        {
            return _userRepository.FindBy(x => x.Email == username).FirstOrDefault();
        }

        public User GetUserByUserNameAndPassword(string username, string hashedPassword)
        {
            var user = _userRepository.FindBy(x => x.Email == username && x.Password == hashedPassword).FirstOrDefault();
            
            return user;
        }
        #endregion
        
        #region User Service
        public bool UpdateUser(User user)
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