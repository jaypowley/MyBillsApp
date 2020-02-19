using MyBills.Domain.Entities;

namespace MyBills.Mvc.Models
{
    public class UserBillsViewModel
    {
        public UserBillSet UserBillSet { get; set; }
        public MonthlyBillSet UserMonthlyBillSet { get; set; }
        public UserDetail UserDetails { get; set; }
    }
}
