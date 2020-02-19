namespace MyBills.Domain.Entities
{
    public sealed class RecurrenceSchedule
    {
        public int Id { get; set; }
        public int RecurrenceTypeId { get; set; }
        public string Schedule { get; set; }
        public RecurrenceType RecurrenceType { get; set; }
    }
}
