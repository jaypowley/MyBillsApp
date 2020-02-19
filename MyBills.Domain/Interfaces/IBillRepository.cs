using System.Collections.Generic;
using MyBills.Domain.Entities;

namespace MyBills.Domain.Interfaces
{
    public interface IBillRepository
    {
        List<Bill> GetUserBills(int userId);

        Bill GetUserBillByBillId(int userId, int billId);

        Bill CreateNewBill(Bill bill);

        void UpdateBill(Bill bill);

        void DeleteUserBillByBillId(int userId, int billId);

    }
}
