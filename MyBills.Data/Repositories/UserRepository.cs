using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyBills.Core;
using MyBills.Data.Contexts;
using MyBills.Domain.Entities;
using MyBills.Domain.Interfaces;

namespace MyBills.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ILogRepository _logRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        public UserRepository()
        {
            this._logRepository = new LogRepository();
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="newUser">The <see cref="User"></see></param>
        #region Private Methods
        private void CreateUser(User newUser)
        {
            try
            {
                string newPass;
                if (newUser.PasswordHash.Trim() == string.Empty)
                {
                    var randomWordPass = GenerateRandomPassword();
                    newPass = Authentication.Compute(randomWordPass);
                }
                else
                {
                    newPass = newUser.PasswordHash;
                }

                var user = new User
                {
                    Username = newUser.Email,
                    Email = newUser.Email,
                    PasswordHash = newPass,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                };

                using (var ctx = new MyBillsContext())
                {
                    ctx.Entry(newUser).State = EntityState.Unchanged;
                    ctx.Users.Add(user);
                    ctx.SaveChanges();

                    newUser.Id = user.Id;
                }

                _logRepository.WriteLog(LogLevel.Debug, "UserRepository.CreateUser", $"New User Created - {user.Email}");
            }
            catch (Exception ex)
            {
                _logRepository.WriteLog(LogLevel.Error, "UserRepository.CreateUser", ex.Message, ex);
            }
        }

        /// <summary>
        /// Generates a random password
        /// </summary>
        /// <returns></returns>
        private string GenerateRandomPassword()
        {
            var generatedPassword = String.Empty;

            try
            {
                var rnd = new Random();
                var first = rnd.Next(1, 200);
                var second = rnd.Next(1, 200);
                var idList = new List<int>
                {
                    first,
                    second
                };

                List<Words> randomWords;
                using (var ctx = new MyBillsContext())
                {
                    randomWords = ctx.Words.Where(e => idList.Contains(e.Id)).ToList();
                }

                generatedPassword = randomWords.Aggregate(generatedPassword, (current, word) => current + (word.Word + "_"));
            }
            catch (Exception ex)
            {
                _logRepository.WriteLog(LogLevel.Error, "User.GenerateRandomPassword", ex.Message, ex);
                generatedPassword = "alpha_omega";
            }

            return generatedPassword.TrimEnd('_');
        }

        /// <summary>
        /// Gets the password hash by the passed username
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns></returns>
        private static string GetPasswordHashByEmail(string username)
        {
            User user;
            using (var ctx = new MyBillsContext())
            {
                user = ctx.Users.First(x => x.Username == username);
            }
            return user.PasswordHash;
        }

        /// <summary>
        /// Gets the user by username
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns></returns>
        private static User GetUserByUsername(string username)
        {
            User user;
            using (var ctx = new MyBillsContext())
            {
                user =  ctx.Users.First(x => x.Username == username);
            }
            return user;
        }

        /// <summary>
        /// Gets the user by username
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns></returns>
        private static async Task<User> GetUserByUsernameAsync(string username)
        {
            User user;
            await using (var ctx = new MyBillsContext())
            {
                user = await ctx.Users.SingleAsync(x => x.Username == username);
            }
            return user;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks if user exists by username
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns></returns>
        public bool FindUserByUsername(string username)
        {
            User user;
            using (var ctx = new MyBillsContext())
            {
                user = ctx.Users.FirstOrDefault(x => x.Username == username);
            }

            return user != null;
        }

        /// <summary>
        /// Checks if user exists by email address
        /// </summary>
        /// <param name="emailAddress">The email address</param>
        /// <returns></returns>
        public bool FindUserByEmailAddress(string emailAddress)
        {
            User user;
            using (var ctx = new MyBillsContext())
            {
                user = ctx.Users.FirstOrDefault(x => x.Email == emailAddress);
            }

            return user != null;
        }

        /// <summary>
        /// Authenticates the user by verifying that supplied password matches persisted password 
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password2">The password</param>
        /// <returns></returns>
        public bool AuthenticateUser(string username, string password2)
        {
            var storedHash = GetPasswordHashByEmail(username);
            return storedHash != null && Authentication.Verify(password2, storedHash);
        }

        /// <summary>
        /// Gets user id by username
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns></returns>
        public int GetUserId(string username)
        {
            var user = GetUserByUsername(username);
            return user.Id;
        }

        /// <summary>
        /// Gets user id by username
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns></returns>
        public async Task<int> GetUserIdAsync(string username)
        {
            var user = await GetUserByUsernameAsync(username);
            return user.Id;
        }

        /// <summary>
        /// Saves the new user information
        /// </summary>
        /// <param name="email">The email address</param>
        /// <param name="password">The password</param>
        /// <param name="friendlyName">The friendly name of the user</param>
        /// <returns></returns>
        public bool RegisterNewUser(string email, string password, string friendlyName)
        {
            var user = new User
            {
                Username = email,
                Email = email,
                PasswordHash = Authentication.Compute(password),
                CreatedDate = DateTime.Now
            };

            try
            {
                CreateUser(user);
                AddDetailsToUser(user, friendlyName);
            }
            catch (Exception ex)
            {
                _logRepository.WriteLog(LogLevel.Error, "UserRepository.RegisterNewUser", ex.Message, ex, email);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the user details by user id
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <returns></returns>
        public async Task<UserDetail> GetUserDetailByUserIdAsync(int userId)
        {
            UserDetail userDetails;
            await using (var ctx = new MyBillsContext())
            {
                userDetails = await (from ud in ctx.UserDetails
                               where ud.User.Id == userId
                               select ud).FirstOrDefaultAsync();
            }

            return userDetails;
        }

        /// <summary>
        /// Add the user details to the registered user
        /// </summary>
        /// <param name="user">The <see cref="User"></see></param>
        /// <param name="friendlyName">The friendly name of the user</param>
        public void AddDetailsToUser(User user, string friendlyName)
        {
            try
            {
                using var ctx = new MyBillsContext();
                var userAccount = ctx.Users.Single(x => x.Id == user.Id);
                var ud = new UserDetail
                {
                    User = userAccount,
                    FirstName = friendlyName
                };

                ctx.UserDetails.Add(ud);
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                _logRepository.WriteLog(LogLevel.Error, "UserRepository.AddDetailsToUser", ex.Message, ex);
            }
        }

        #endregion
    }
}
