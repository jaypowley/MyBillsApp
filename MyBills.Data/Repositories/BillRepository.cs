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
        /// <summary>
        /// Gets user bills by user id
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <returns></returns>
        public List<Bill> GetUserBills(int userId)
        {
            List<Bill> filteredList;
            using (var ctx = new MyBillsContext())
            {
                var bills = (from ub in ctx.UserBills
                                    join bill in ctx.Bills on ub.Bill equals bill
                                    where ub.User.Id == userId
                                    select bill).ToList();

                //Filter out dupes
                filteredList = bills.GroupBy(x => x.Id)
                                    .Select(grp => grp.First())
                                    .ToList();
            }

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
            using (var ctx = new MyBillsContext())
            {
                var bills = (from ub in ctx.UserBills
                                    join userBill in ctx.Bills on ub.Bill equals userBill
                                    join user in ctx.Users on ub.User.Id equals userId
                                    where ub.Bill.Id == billId
                                    select userBill).ToList();

                //Filter out dupes
                filteredList = bills.GroupBy(x => x.Id)
                                    .Select(grp => grp.First())
                                    .ToList();
            }

            return filteredList.FirstOrDefault();
        }

        /// <summary>
        /// Creates a new bill
        /// </summary>
        /// <param name="bill">The new bill to create</param>
        /// <returns></returns>
        public Bill CreateNewBill(Bill bill)
        {
            using var ctx = new MyBillsContext();
            var newBill = new Bill
            {
                Name = bill.Name,
                Amount = bill.Amount,
                IsAutoPaid = bill.IsAutoPaid,
                IsComplete = bill.IsComplete
            };

            ctx.Bills.Add(newBill);

            ctx.SaveChanges();

            return newBill;
        }
        
        /// <summary>
        /// Updates an existing bill
        /// </summary>
        /// <param name="bill">The bill to update</param>
        public void UpdateBill(Bill bill)
        {
            using var ctx = new MyBillsContext();
            ctx.Entry(bill).State = EntityState.Modified;
            ctx.SaveChanges();
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

            using var ctx = new MyBillsContext();
            var userBill = ctx.UserBills.FirstOrDefault(x => x.BillId == bill.Id);

            if (userBill == null) return;

            var userBillRecurrenceScheduleId = userBill.RecurrenceScheduleId;
            ctx.UserBills.RemoveRange(ctx.UserBills.Where(x => x.BillId == bill.Id).AsEnumerable());
            ctx.UserBillRecurrenceSchedule.RemoveRange(ctx.UserBillRecurrenceSchedule.Where(x => x.Id == userBillRecurrenceScheduleId).AsEnumerable());
            ctx.Bills.Attach(bill);
            ctx.Bills.Remove(bill);
            ctx.SaveChanges();
        }

    }
}
