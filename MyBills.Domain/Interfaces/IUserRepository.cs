using System.Threading.Tasks;
using MyBills.Domain.Entities;

namespace MyBills.Domain.Interfaces
{
    public interface IUserRepository
    {
        bool FindUserByUsername(string username);

        bool FindUserByEmailAddress(string emailAddress);

        bool AuthenticateUser(string username, string password);

        int GetUserId(string name);

        Task<int> GetUserIdAsync(string name);

        bool RegisterNewUser(string email, string password, string friendlyName);

        Task<UserDetail> GetUserDetailByUserIdAsync(int userId);

        void AddDetailsToUser(User user, string friendlyName);
    }
}
