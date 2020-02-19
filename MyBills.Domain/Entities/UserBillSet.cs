using System.Collections.Generic;

namespace MyBills.Domain.Entities
{
    public class UserBillSet
    {
        public int UserId { get; set; }
        public List<UserBillDetail> BillDetails { get; set; }
        public decimal RemainingBalance { get; set; }
    }
}
