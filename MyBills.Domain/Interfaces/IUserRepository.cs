using System.Threading.Tasks;
using MyBills.Domain.Entities;

namespace MyBills.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> FindUserByUsernameAsync(string username);

        Task<bool> FindUserByEmailAddressAsync(string emailAddress);

        Task<bool> AuthenticateUserAsync(string username, string password);

        Task<int> GetUserIdAsync(string name);

        Task<bool> RegisterNewUserAsync(string email, string password, string friendlyName);

        Task<UserDetail> GetUserDetailByUserIdAsync(int userId);

        Task AddDetailsToUserAsync(User user, string friendlyName);
    }
}
