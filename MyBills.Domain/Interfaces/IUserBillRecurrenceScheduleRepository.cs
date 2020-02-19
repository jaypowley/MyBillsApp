using MyBills.Domain.Entities;

namespace MyBills.Domain.Interfaces
{
    public interface IUserBillRecurrenceScheduleRepository
    {
        RecurrenceSchedule GetRecSchedule(int recTypeId, IRecurrenceModel recModel);

        RecurrenceSchedule CreateNewRecurrenceSchedule(int recurrenceTypeId, string schedule);
    }
}
