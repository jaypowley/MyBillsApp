using System.ComponentModel;
using MyBills.Domain.Interfaces;

namespace MyBills.Domain.Entities
{
    public class UserBillDetail
    {
        public int UserId { get; set; }
        
        public Bill Bill { get; set; }
        
        public int BillId { get; set; }
        
        [DisplayName("Bill Name")]
        public string BillName { get; set; }
        
        public decimal Amount { get; set; }
        
        public int Month { get; set; }
        
        public int Year { get; set; }
        
        [DisplayName("Is Complete?")]
        public bool IsComplete { get; set; }
        
        [DisplayName("Is Auto Paid?")]
        public bool IsAutoPaid { get; set; }
        
        [DisplayName("Recurrence Type")]
        public string RecurrenceTypeName { get; set; }
        
        public int RecurrenceTypeId { get; set; }
        
        public string Schedule { get; set; }
        
        public int RecurrenceScheduleId { get; set; }
        
        public RecurrenceSchedule RecurrenceSchedule { get; set; }
        
        public IRecurrenceModel RecModel { get; set; }
    }
}
