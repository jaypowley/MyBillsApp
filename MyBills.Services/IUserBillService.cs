using MyBills.Domain.Entities;
using MyBills.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBills.Services
{
    public interface IUserBillService
    {
        MonthlyBillSet GetMonthlyBillSetByUserIdAndMonthYear(int userId, int month, int year);
        Bill GetUserBillByBillId(int userId, int billId);
        Task<List<RecurrenceType>> GetRecurrenceTypes();
        void MarkBillAsPaid(int billId, int userId, int day, int month, int year);
        UserBillSet GetBillsByUserIdConsolidated(int userId);
        bool CreateNewUserBill(int userId, Bill bill, IRecurrenceModel recModel, RecurrenceSchedule recSchedule);
        RecurrenceSchedule GetRecSchedule(int recurrenceTypeId, IRecurrenceModel recModel);
        bool UpdateUserBill(Bill bill);
        void DeleteUserBillByBillId(int userId, int billId);

    }
}