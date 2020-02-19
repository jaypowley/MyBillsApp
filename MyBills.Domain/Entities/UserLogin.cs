namespace MyBills.Domain.Entities
{
    public class UserLogin
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsSuccess { get; set; }
    }
}
