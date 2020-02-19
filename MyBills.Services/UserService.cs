using System.Threading.Tasks;
using MyBills.Core;
using MyBills.Data.Repositories;
using MyBills.Domain.Entities;
using MyBills.Domain.Interfaces;

namespace MyBills.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        public UserService()
        {
            _userRepository = new UserRepository();
        }

        /// <summary>
        /// Gets the user id by username.
        /// </summary>
        /// <param name="userName">The username</param>
        /// <returns></returns>
        public async Task<int> GetUserId(string userName)
        {
            var userId = await AppCache<int>.GetOrCreate("userKey" + userName, async () => await _userRepository.GetUserIdAsync(userName));
            return userId;
        }

        /// <summary>
        /// Gets the user detail by user id.
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <returns></returns>
        public async Task<UserDetail> GetUserDetailByUserId(int userId)
        {
            var userDetail = await AppCache<UserDetail>.GetOrCreate("userDetail" + userId, async () => await _userRepository.GetUserDetailByUserIdAsync(userId));
            return userDetail;
        }
    }
}
