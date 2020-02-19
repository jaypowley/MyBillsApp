using System;
using MyBills.Core;
using MyBills.Data.Repositories;
using MyBills.Domain.Interfaces;

namespace MyBills.Services
{
    public class LoginRegisterService
    {
        private readonly ILogRepository _logRepository;
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginRegisterService"/> class.
        /// </summary>
        public LoginRegisterService()
        {
            this._logRepository = new LogRepository();
            this._userRepository = new UserRepository();
        }

        /// <summary>
        /// Validates the supplied username and password.
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <returns></returns>
        public bool Login(string username, string password)
        {
            try
            {
                var isSuccess = _userRepository.FindUserByUsername(username);
                if (isSuccess) return _userRepository.AuthenticateUser(username, password);
            }
            catch (Exception ex)
            {
                _logRepository.WriteLog(LogLevel.Error, "LoginRegisterService.Login", ex.Message, ex, username);
                return false;
            }

            return false;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="email">The email address</param>
        /// <param name="password">The password</param>
        /// <param name="friendlyName">The friendly name of the user</param>
        /// <returns></returns>
        public bool RegisterNewUser(string email, string password, string friendlyName)
        {
            try
            {
                var isSuccess = _userRepository.RegisterNewUser(email, password, friendlyName);
                if (isSuccess) return _userRepository.AuthenticateUser(email, password);
            }
            catch (Exception ex)
            {
                _logRepository.WriteLog(LogLevel.Error, "LoginRegisterService.RegisterNewUser", ex.Message, ex, email);
                return false;
            }

            return false;
        }

        /// <summary>
        /// Validates the registration information.
        /// </summary>
        /// <param name="password">The password</param>
        /// <param name="confirmPassword">The confirmation password</param>
        /// <param name="email">The email address</param>
        /// <param name="friendlyName">The friendly name of the user</param>
        /// <returns></returns>
        public static bool IsRegistrationValid(string password, string confirmPassword, string email, string friendlyName)
        {
            if (!string.IsNullOrEmpty(password))
            {
                if (string.IsNullOrEmpty(password.Trim()))
                    return false;
            }
            else
                return false;

            if (!string.IsNullOrEmpty(password))
            {
                if (string.IsNullOrEmpty(password.Trim()))
                    return false;
            }
            else
                return false;

            if (password != confirmPassword)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(email))
            {
                if (string.IsNullOrEmpty(email.Trim()))
                    return false;
            }
            else
                return false;

            if (!string.IsNullOrEmpty(friendlyName))
            {
                if (string.IsNullOrEmpty(friendlyName.Trim()))
                    return false;
            }
            else
                return false;

            return true;
        }

        /// <summary>
        /// Validates the login information.
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <returns></returns>
        public static bool IsLoginValid(string username, string password)
        {
            if (!string.IsNullOrEmpty(username))
            {
                if (string.IsNullOrEmpty(username.Trim()))
                    return false;
            }
            else
                return false;

            if (!string.IsNullOrEmpty(password))
            {
                if (string.IsNullOrEmpty(password.Trim()))
                    return false;
            }
            else
                return false;

            return true;
        }
    }
}
