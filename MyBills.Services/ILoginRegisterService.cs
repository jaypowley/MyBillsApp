using System.Threading.Tasks;

namespace MyBills.Services
{
    public interface ILoginRegisterService
    {
        Task<bool> LoginAsync(string username, string password);
        Task<bool> RegisterNewUserAsync(string email, string password, string friendlyName);
    }
}