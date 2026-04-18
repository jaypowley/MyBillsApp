namespace MyBills.Services
{
    public interface ILoginRegisterService
    {
        bool Login(string username, string password);
        bool RegisterNewUser(string email, string password, string friendlyName);
    }
}