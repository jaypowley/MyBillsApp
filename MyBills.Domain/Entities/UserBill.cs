namespace MyBills.Domain.Entities
{
    public sealed class UserBill
    {
        public int Id { get; set; }
        public int BillId { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public bool IsPaid { get; set; }
        public int UserId { get; set; }
        public int RecurrenceScheduleId { get; set; }

        public Bill Bill { get; set; }
        public User User { get; set; }
        public RecurrenceSchedule RecurrenceSchedule { get; set; }
    }
}
