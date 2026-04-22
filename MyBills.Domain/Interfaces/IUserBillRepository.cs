using System.Collections.Generic;
using System.Threading.Tasks;
using MyBills.Domain.Entities;

namespace MyBills.Domain.Interfaces
{
    public interface IUserBillRepository
    {
        Task MarkBillAsPaidAsync(int billId, int userId, int day, int month, int year);

        Task<UserBillSet> GetBillsByUserIdConsolidatedAsync(int userId);

        Task<List<UserBill>> GetBillsByUserIdAndMonthYearAsync(int userId, int month, int year);

        Task<List<UserBill>> GenerateRecurringBillsAsync(int userId, int month, int year);

        Task CreateNewUserBillAsync(int userId, Bill bill, IRecurrenceModel model, RecurrenceSchedule recurrenceSchedule);
    }
}
