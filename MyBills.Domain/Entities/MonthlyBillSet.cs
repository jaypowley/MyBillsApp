using System.Collections.Generic;
using System.ComponentModel;

namespace MyBills.Domain.Entities
{
    public class MonthlyBillSet
    {
        public int UserId { get; set; }
        public List<UserBill> UserBills { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        [DisplayName("Remaining Balance")]
        public decimal RemainingBalance { get; set; }
    }
}
