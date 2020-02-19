using System.Collections.Generic;
using MyBills.Domain.Entities;

namespace MyBills.Domain.Interfaces
{
    public interface IUserBillRepository
    {
        void MarkBillAsPaid(int billId, int userId, int day, int month, int year);

        UserBillSet GetBillsByUserIdConsolidated(int userId);

        List<UserBill> GetBillsByUserIdAndMonthYear(int userId, int month, int year);

        List<UserBill> GenerateRecurringBills(int userId, int month, int year);

        void CreateNewUserBill(int userId, Bill bill, IRecurrenceModel model, RecurrenceSchedule recurrenceSchedule);
    }
}
