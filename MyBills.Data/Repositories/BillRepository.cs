using MyBills.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyBills.Data.Contexts;
using MyBills.Domain.Entities;

namespace MyBills.Data.Repositories
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BillRepository"/> class.
    /// </summary>
    public class BillRepository : IBillRepository
    {
        private readonly MyBillsContext _context;
        
        public BillRepository(MyBillsContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets user bills by user id
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <returns></returns>
        public async Task<List<Bill>> GetUserBillsAsync(int userId)
        {
            List<Bill> filteredList;
            var bills = await (from ub in _context.UserBills
                               join bill in _context.Bills on ub.Bill equals bill
                               where ub.User.Id == userId
                               select bill).ToListAsync();

            //Filter out dupes
            filteredList = bills.GroupBy(x => x.Id)
                                .Select(grp => grp.First())
                                .ToList();

            return filteredList;
        }

        /// <summary>
        /// Get a user bill entity by user id and bill id
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="billId">The bill id</param>
        /// <returns></returns>
        public async Task<Bill> GetUserBillByBillIdAsync(int userId, int billId)
        {
            List<Bill> filteredList;
            var bills = await (from ub in _context.UserBills
                               join userBill in _context.Bills on ub.Bill equals userBill
                               join user in _context.Users on ub.User.Id equals userId
                               where ub.Bill.Id == billId
                               select userBill).ToListAsync();

            //Filter out dupes
            filteredList = bills.GroupBy(x => x.Id)
                                .Select(grp => grp.First())
                                .ToList();

            return filteredList.FirstOrDefault();
        }

        /// <summary>
        /// Creates a new bill
        /// </summary>
        /// <param name="bill">The new bill to create</param>
        /// <returns></returns>
        public async Task<Bill> CreateNewBillAsync(Bill bill)
        {            
            var newBill = new Bill
            {
                Name = bill.Name,
                Amount = bill.Amount,
                IsAutoPaid = bill.IsAutoPaid,
                IsComplete = bill.IsComplete
            };

            _context.Bills.Add(newBill);

            await _context.SaveChangesAsync();

            return newBill;
        }
        
        /// <summary>
        /// Updates an existing bill
        /// </summary>
        /// <param name="bill">The bill to update</param>
        public async Task UpdateBillAsync(Bill bill)
        {            
            _context.Entry(bill).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a bill by user id and bill id
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="billId">The bill id</param>
        public async Task DeleteUserBillByBillIdAsync(int userId, int billId)
        {
            var bill = await GetUserBillByBillIdAsync(userId, billId);
            if (bill == null) return;
            
            var userBill = await _context.UserBills.FirstOrDefaultAsync(x => x.BillId == bill.Id);

            if (userBill == null) return;

            var userBillRecurrenceScheduleId = userBill.RecurrenceScheduleId;
            _context.UserBills.RemoveRange(_context.UserBills.Where(x => x.BillId == bill.Id));
            _context.UserBillRecurrenceSchedule.RemoveRange(_context.UserBillRecurrenceSchedule.Where(x => x.Id == userBillRecurrenceScheduleId));
            _context.Bills.Attach(bill);
            _context.Bills.Remove(bill);
            await _context.SaveChangesAsync();
        }

    }
}
