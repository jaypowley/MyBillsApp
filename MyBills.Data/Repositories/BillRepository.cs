using MyBills.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
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
        public List<Bill> GetUserBills(int userId)
        {
            List<Bill> filteredList;
            var bills = (from ub in _context.UserBills
                         join bill in _context.Bills on ub.Bill equals bill
                         where ub.User.Id == userId
                         select bill).ToList();

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
        public Bill GetUserBillByBillId(int userId, int billId)
        {
            List<Bill> filteredList;
            var bills = (from ub in _context.UserBills
                         join userBill in _context.Bills on ub.Bill equals userBill
                         join user in _context.Users on ub.User.Id equals userId
                         where ub.Bill.Id == billId
                         select userBill).ToList();

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
        public Bill CreateNewBill(Bill bill)
        {            
            var newBill = new Bill
            {
                Name = bill.Name,
                Amount = bill.Amount,
                IsAutoPaid = bill.IsAutoPaid,
                IsComplete = bill.IsComplete
            };

            _context.Bills.Add(newBill);

            _context.SaveChanges();

            return newBill;
        }
        
        /// <summary>
        /// Updates an existing bill
        /// </summary>
        /// <param name="bill">The bill to update</param>
        public void UpdateBill(Bill bill)
        {            
            _context.Entry(bill).State = EntityState.Modified;
            _context.SaveChanges();
        }

        /// <summary>
        /// Deletes a bill by user id and bill id
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="billId">The bill id</param>
        public void DeleteUserBillByBillId(int userId, int billId)
        {
            var bill = GetUserBillByBillId(userId, billId);
            if (bill == null) return;
            
            var userBill = _context.UserBills.FirstOrDefault(x => x.BillId == bill.Id);

            if (userBill == null) return;

            var userBillRecurrenceScheduleId = userBill.RecurrenceScheduleId;
            _context.UserBills.RemoveRange(_context.UserBills.Where(x => x.BillId == bill.Id).AsEnumerable());
            _context.UserBillRecurrenceSchedule.RemoveRange(_context.UserBillRecurrenceSchedule.Where(x => x.Id == userBillRecurrenceScheduleId).AsEnumerable());
            _context.Bills.Attach(bill);
            _context.Bills.Remove(bill);
            _context.SaveChanges();
        }

    }
}
