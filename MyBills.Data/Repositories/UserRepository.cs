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
        private readonly MyBillsContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        public UserRepository(MyBillsContext ctx)
        {
            _context = ctx;
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


                _context.Entry(newUser).State = EntityState.Unchanged;
                _context.Users.Add(user);
                _context.SaveChanges();

                newUser.Id = user.Id;
            }
            catch (Exception ex)
            {
                //TODO: Log the exception
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

                List<Words> randomWords = _context.Words.Where(e => idList.Contains(e.Id)).ToList();

                generatedPassword = randomWords.Aggregate(generatedPassword, (current, word) => current + (word.Word + "_"));
            }
            catch (Exception ex)
            {
                //TODO: Log the exception
                generatedPassword = "alpha_omega";
            }

            return generatedPassword.TrimEnd('_');
        }

        /// <summary>
        /// Gets the password hash by the passed username
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns></returns>
        private string GetPasswordHashByEmail(string username)
        {
            User user = _context.Users.First(x => x.Username == username);
            
            return user.PasswordHash;
        }

        /// <summary>
        /// Gets the user by username
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns></returns>
        private User GetUserByUsername(string username)
        {
            User user = _context.Users.First(x => x.Username == username);
            
            return user;
        }

        /// <summary>
        /// Gets the user by username
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns></returns>
        private async Task<User> GetUserByUsernameAsync(string username)
        {
            User user = await _context.Users.SingleOrDefaultAsync(x => x.Username == username);

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
            User user = _context.Users.FirstOrDefault(x => x.Username == username);
            
            return user != null;
        }

        /// <summary>
        /// Checks if user exists by email address
        /// </summary>
        /// <param name="emailAddress">The email address</param>
        /// <returns></returns>
        public bool FindUserByEmailAddress(string emailAddress)
        {
            User user = _context.Users.FirstOrDefault(x => x.Email == emailAddress);

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
                //TODO: Log the exception
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
              UserDetail userDetail = await (from ud in _context.UserDetails
                                           where ud.UserId == userId
                                           select ud).FirstAsync();


            return userDetail;
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
                var userAccount = _context.Users.Single(x => x.Id == user.Id);
                var ud = new UserDetail
                {
                    User = userAccount,
                    FirstName = friendlyName
                };

                _context.UserDetails.Add(ud);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                //TODO: Log the exception
            }
        }

        #endregion
    }
}
