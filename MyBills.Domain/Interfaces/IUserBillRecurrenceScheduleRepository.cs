using System.Threading.Tasks;
using MyBills.Domain.Entities;

namespace MyBills.Domain.Interfaces
{
    public interface IUserBillRecurrenceScheduleRepository
    {
        Task<RecurrenceSchedule> GetRecScheduleAsync(int recTypeId, IRecurrenceModel recModel);

        Task<RecurrenceSchedule> CreateNewRecurrenceScheduleAsync(int recurrenceTypeId, string schedule);
    }
}
