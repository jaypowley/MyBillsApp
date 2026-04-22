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

        #region Private Methods
        private async Task CreateUserAsync(User newUser)
        {
            try
            {
                string newPass;
                if (newUser.PasswordHash.Trim() == string.Empty)
                {
                    var randomWordPass = await GenerateRandomPasswordAsync();
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
                await _context.SaveChangesAsync();

                newUser.Id = user.Id;
            }
            catch (Exception ex)
            {
                //TODO: Log the exception
            }
        }

        private async Task<string> GenerateRandomPasswordAsync()
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

                var randomWords = await _context.Words.Where(e => idList.Contains(e.Id)).ToListAsync();

                generatedPassword = randomWords.Aggregate(generatedPassword, (current, word) => current + (word.Word + "_"));
            }
            catch (Exception ex)
            {
                //TODO: Log the exception
                generatedPassword = "alpha_omega";
            }

            return generatedPassword.TrimEnd('_');
        }

        private async Task<string> GetPasswordHashByEmailAsync(string username)
        {
            return await _context.Users
                .Where(x => x.Username == username)
                .Select(x => x.PasswordHash)
                .FirstOrDefaultAsync();
        }

        private async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.SingleOrDefaultAsync(x => x.Username == username);
        }

        #endregion

        #region Public Methods

        public async Task<bool> FindUserByUsernameAsync(string username)
        {
            return await _context.Users.AnyAsync(x => x.Username == username);
        }

        public async Task<bool> FindUserByEmailAddressAsync(string emailAddress)
        {
            return await _context.Users.AnyAsync(x => x.Email == emailAddress);
        }

        public async Task<bool> AuthenticateUserAsync(string username, string password)
        {
            var storedHash = await GetPasswordHashByEmailAsync(username);
            return storedHash != null && Authentication.Verify(password, storedHash);
        }

        public async Task<int> GetUserIdAsync(string username)
        {
            var user = await GetUserByUsernameAsync(username);
            return user.Id;
        }

        public async Task<bool> RegisterNewUserAsync(string email, string password, string friendlyName)
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
                await CreateUserAsync(user);
                await AddDetailsToUserAsync(user, friendlyName);
            }
            catch (Exception ex)
            {
                //TODO: Log the exception
                return false;
            }

            return true;
        }

        public async Task<UserDetail> GetUserDetailByUserIdAsync(int userId)
        {
            return await (from ud in _context.UserDetails
                          where ud.UserId == userId
                          select ud).FirstAsync();
        }

        public async Task AddDetailsToUserAsync(User user, string friendlyName)
        {
            try
            {
                var userAccount = await _context.Users.SingleAsync(x => x.Id == user.Id);
                var ud = new UserDetail
                {
                    User = userAccount,
                    FirstName = friendlyName
                };

                _context.UserDetails.Add(ud);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                //TODO: Log the exception
            }
        }

        #endregion
    }
}
