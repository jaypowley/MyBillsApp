using System.Collections.Generic;
using System.Threading.Tasks;
using MyBills.Domain.Entities;

namespace MyBills.Domain.Interfaces
{
    public interface IBillRepository
    {
        Task<List<Bill>> GetUserBillsAsync(int userId);

        Task<Bill> GetUserBillByBillIdAsync(int userId, int billId);

        Task<Bill> CreateNewBillAsync(Bill bill);

        Task UpdateBillAsync(Bill bill);

        Task DeleteUserBillByBillIdAsync(int userId, int billId);

    }
}
