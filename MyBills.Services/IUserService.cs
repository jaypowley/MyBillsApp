using MyBills.Domain.Entities;
using System.Threading.Tasks;

namespace MyBills.Services
{
    public interface IUserService
    {
        Task<int> GetUserId(string userName);

        Task<UserDetail> GetUserDetailByUserId(int userId);
    }
}