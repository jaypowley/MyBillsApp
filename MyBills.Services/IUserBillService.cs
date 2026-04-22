using MyBills.Domain.Entities;
using MyBills.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBills.Services
{
    public interface IUserBillService
    {
        Task<MonthlyBillSet> GetMonthlyBillSetByUserIdAndMonthYearAsync(int userId, int month, int year);
        Task<Bill> GetUserBillByBillIdAsync(int userId, int billId);
        Task<List<RecurrenceType>> GetRecurrenceTypes();
        Task MarkBillAsPaidAsync(int billId, int userId, int day, int month, int year);
        Task<UserBillSet> GetBillsByUserIdConsolidatedAsync(int userId);
        Task<bool> CreateNewUserBillAsync(int userId, Bill bill, IRecurrenceModel recModel, RecurrenceSchedule recSchedule);
        Task<RecurrenceSchedule> GetRecScheduleAsync(int recurrenceTypeId, IRecurrenceModel recModel);
        Task<bool> UpdateUserBillAsync(Bill bill);
        Task DeleteUserBillByBillIdAsync(int userId, int billId);

    }
}