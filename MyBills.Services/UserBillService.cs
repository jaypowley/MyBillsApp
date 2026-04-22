using MyBills.Core;
using MyBills.Domain.Entities;
using MyBills.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBills.Services
{
    public class UserBillService: IUserBillService
    {        
        private readonly IBillRepository _billRepository;
        private readonly IUserBillRepository _userBillRepository;
        private readonly IRecurrenceTypeRepository _recurrenceTypeRepository;
        private readonly IUserBillRecurrenceScheduleRepository _userBillRecurrenceScheduleRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserBillService"/> class.
        /// </summary>
        public UserBillService(IBillRepository billRepository, IUserBillRepository userBillRepository, IRecurrenceTypeRepository recurrenceTypeRepository, IUserBillRecurrenceScheduleRepository userBillRecurrenceScheduleRepository)
        {            
            _billRepository = billRepository;
            _userBillRepository = userBillRepository;
            _recurrenceTypeRepository = recurrenceTypeRepository;
            _userBillRecurrenceScheduleRepository = userBillRecurrenceScheduleRepository;
        }

        /// <summary>
        /// Gets the monthly bill set (view model) by user id, month, and year.
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="month">The month</param>
        /// <param name="year">The year</param>
        /// <returns></returns>
        public async Task<MonthlyBillSet> GetMonthlyBillSetByUserIdAndMonthYearAsync(int userId, int month, int year)
        {
            var userBills = await _userBillRepository.GetBillsByUserIdAndMonthYearAsync(userId, month, year);

            userBills = !userBills.Any() ? await _userBillRepository.GenerateRecurringBillsAsync(userId, month, year) : userBills;

            var mbs = new MonthlyBillSet()
            {
                UserId = userId,
                Month = month,
                Year = year,
                UserBills = userBills,
                RemainingBalance = userBills.Where(y => y.IsPaid == false).Sum(x => x.Bill.Amount)
            };

            return mbs;
        }

        /// <summary>
        /// Gets the user bill by user id and bill id.
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="billId">The bill id</param>
        /// <returns></returns>
        public async Task<Bill> GetUserBillByBillIdAsync(int userId, int billId)
        {
            return await _billRepository.GetUserBillByBillIdAsync(userId, billId);
        }

        /// <summary>
        /// Gets the recurrence types
        /// </summary>
        /// <returns></returns>
        public async Task<List<RecurrenceType>> GetRecurrenceTypes()
        {
            var recurrenceTypes = await AppCache<List<RecurrenceType>>.GetOrCreate("recurrenceTypes", async () => await _recurrenceTypeRepository.GetRecurrenceTypesAsync());

            return recurrenceTypes;
        }

        /// <summary>
        /// Updates the bill record as paid or unpaid.
        /// </summary>
        /// <param name="billId">The bill id</param>
        /// <param name="userId">The user id</param>
        /// <param name="day">The day</param>
        /// <param name="month">The month</param>
        /// <param name="year">The year</param>
        public async Task MarkBillAsPaidAsync(int billId, int userId, int day, int month, int year)
        {
            await _userBillRepository.MarkBillAsPaidAsync(billId, userId, day, month, year);
        }

        /// <summary>
        /// Gets the bill collection by user id.
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <returns></returns>
        public async Task<UserBillSet> GetBillsByUserIdConsolidatedAsync(int userId)
        {
            return await _userBillRepository.GetBillsByUserIdConsolidatedAsync(userId);
        }

        /// <summary>
        /// Gets the text for the previous and next month navigation buttons.
        /// </summary>
        /// <param name="month">The month</param>
        /// <param name="year">The year</param>
        /// <returns></returns>
        public static NavigationHelper GetNavigationHelper(int? month, int? year)
        {
            var navHelper = new NavigationHelper();
            var givenMonth = month.GetValueOrDefault();
            var givenYear = year.GetValueOrDefault();

            //Calculates previous and next months from passed in month
            if (givenMonth != 0 && givenYear != 0)
            {
                navHelper.CurrentMonth = givenMonth;
                navHelper.CurrentYear = givenYear;
                navHelper.PrevMonth = new DateTime(givenYear, givenMonth, 1).AddMonths(-1).Month;
                navHelper.PrevMonthsYear = new DateTime(givenYear, givenMonth, 1).AddMonths(-1).Year;
                navHelper.NextMonth = new DateTime(givenYear, givenMonth, 1).AddMonths(1).Month;
                navHelper.NextMonthsYear = new DateTime(givenYear, givenMonth, 1).AddMonths(1).Year;
            }
            else
            {
                navHelper.CurrentMonth = DateTime.Now.Month;
                navHelper.CurrentYear = DateTime.Now.Year;
                navHelper.PrevMonth = navHelper.Today.AddMonths(-1).Month;
                navHelper.PrevMonthsYear = navHelper.Today.AddMonths(-1).Year;
                navHelper.NextMonth = navHelper.Today.AddMonths(1).Month;
                navHelper.NextMonthsYear = navHelper.Today.AddMonths(1).Year;
            }

            return navHelper;
        }

        /// <summary>
        /// Creates a new user bill.
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="bill">The bill object</param>
        /// <param name="recModel">The recurrence model</param>
        /// <param name="recSchedule">The recurrence schedule</param>
        /// <returns></returns>
        public async Task<bool> CreateNewUserBillAsync(int userId, Bill bill, IRecurrenceModel recModel, RecurrenceSchedule recSchedule)
        {
            try
            {
                var newBill = await _billRepository.CreateNewBillAsync(bill);

                var newRecSchedule = await _userBillRecurrenceScheduleRepository.CreateNewRecurrenceScheduleAsync(recSchedule.RecurrenceTypeId, recSchedule.Schedule);

                await _userBillRepository.CreateNewUserBillAsync(userId, newBill, recModel, newRecSchedule);
            }
            catch (Exception ex)
            {
                //TODO: Log the exception
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the recurrence schedule.
        /// </summary>
        /// <param name="recurrenceTypeId">The recurrence type id</param>
        /// <param name="recModel">The recurrence model</param>
        /// <returns></returns>
        public async Task<RecurrenceSchedule> GetRecScheduleAsync(int recurrenceTypeId, IRecurrenceModel recModel)
        {
            return await _userBillRecurrenceScheduleRepository.GetRecScheduleAsync(recurrenceTypeId, recModel);
        }

        /// <summary>
        /// Updates the user bill.
        /// </summary>
        /// <param name="bill">The bill object</param>
        /// <returns></returns>
        public async Task<bool> UpdateUserBillAsync(Bill bill)
        {
            try
            {
                await _billRepository.UpdateBillAsync(bill);
            }
            catch (Exception ex)
            {
                //TODO: Log the exception
                return false;
            }

            return true;
        }

        /// <summary>
        /// Deletes the user bill.
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="billId">The bill id</param>
        public async Task DeleteUserBillByBillIdAsync(int userId, int billId)
        {
            try
            {
                await _billRepository.DeleteUserBillByBillIdAsync(userId, billId);
            }
            catch (Exception ex)
            {
                //TODO: Log the exception
            }
        }

        /// <summary>
        /// Gets the recurrence model of the current bill view model.
        /// </summary>
        /// <param name="billViewModel">The bill view model</param>
        /// <returns></returns>
        public static IRecurrenceModel GetRecModel(BillViewModel billViewModel)
        {
            var selectedRec = billViewModel.RecurrenceTypeId;

            return selectedRec switch
            {
                1 => billViewModel.DailyRecurrence,
                2 => billViewModel.WeeklyRecurrence,
                3 => billViewModel.BiWeeklyEvenRecurrence,
                4 => billViewModel.BiWeeklyOddRecurrence,
                5 => billViewModel.BiMonthlyRecurrence,
                6 => billViewModel.MonthlyRecurrence,
                7 => billViewModel.QuarterlyRecurrence,
                8 => billViewModel.BiYearlyRecurrence,
                9 => billViewModel.YearlyRecurrence,
                10 => billViewModel.OnetimeRecurrence,
                _ => billViewModel.DailyRecurrence
            };
        }
    }
}
