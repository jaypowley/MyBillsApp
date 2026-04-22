using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyBills.Data.Contexts;
using MyBills.Domain.Entities;
using MyBills.Domain.Interfaces;

namespace MyBills.Data.Repositories
{
    public class UserBillRepository : IUserBillRepository
    {
        private readonly MyBillsContext _context;
        private static Calendar Cal => CultureInfo.InvariantCulture.Calendar;

        public UserBillRepository(MyBillsContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Marks a bill a paid
        /// </summary>
        /// <param name="billId">The bill id</param>
        /// <param name="userId">The user id</param>
        /// <param name="day">The bill day</param>
        /// <param name="month">The bill month</param>
        /// <param name="year">The bill year</param>
        public async Task MarkBillAsPaidAsync(int billId, int userId, int day, int month, int year)
        {
            var bill = await _context.UserBills.SingleOrDefaultAsync(x => x.BillId == billId && x.User.Id == userId && x.Day == day && x.Month == month && x.Year == year);
            if (bill == null) return;

            var newValue = !bill.IsPaid;
            bill.IsPaid = newValue;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Gets the user bill set model by user Id
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <returns></returns>
        public async Task<UserBillSet> GetBillsByUserIdConsolidatedAsync(int userId)
        {
            var userBillDetails = await (from ub in _context.UserBills
                                         join bill in _context.Bills on ub.Bill equals bill
                                         join ubrs in _context.UserBillRecurrenceSchedule on ub.RecurrenceSchedule equals ubrs
                                         join rt in _context.RecurrenceType on ubrs.RecurrenceType equals rt
                                         where ub.User.Id == userId
                                         select new UserBillDetail
                                         {
                                             UserId = userId,
                                             Bill = bill,
                                             BillId = bill.Id,
                                             BillName = bill.Name,
                                             Amount = bill.Amount,
                                             Month = ub.Month,
                                             Year = ub.Year,
                                             IsComplete = bill.IsComplete,
                                             IsAutoPaid = bill.IsAutoPaid,
                                             RecurrenceTypeName = rt.Name,
                                             RecurrenceTypeId = ubrs.RecurrenceTypeId,
                                             Schedule = ubrs.Schedule
                                         }).ToListAsync();

            var filteredList = userBillDetails.GroupBy(x => x.BillId)
                                              .Select(grp => grp.First())
                                              .ToList();

            return new UserBillSet
            {
                UserId = userId,
                BillDetails = filteredList
            };
        }

        /// <summary>
        /// Create a new user bill
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="bill">The bill model</param>
        /// <param name="model">The recurrence model</param>
        /// <param name="recurrenceSchedule">The recurrence schedule</param>
        public async Task CreateNewUserBillAsync(int userId, Bill bill, IRecurrenceModel model, RecurrenceSchedule recurrenceSchedule)
        {
            var billDetail = new UserBillDetail
            {
                UserId = userId,
                BillId = bill.Id,
                Month = DateTime.Today.Month,
                Year = DateTime.Today.Year,
                RecurrenceTypeName = recurrenceSchedule.RecurrenceType.Name,
                RecurrenceTypeId = recurrenceSchedule.RecurrenceTypeId,
                Schedule = recurrenceSchedule.Schedule,
                RecurrenceScheduleId = recurrenceSchedule.Id
            };

            await CreateUserBillsAsync(_context, model, billDetail);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Gets all bills for the user and month and year
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="month">The month</param>
        /// <param name="year">The year</param>
        /// <returns></returns>
        public async Task<List<UserBill>> GetBillsByUserIdAndMonthYearAsync(int userId, int month, int year)
        {
            return await _context.UserBills
                .Include(x => x.Bill)
                .Where(x => x.UserId == userId && x.Month == month && x.Year == year && x.Bill.IsComplete == false)
                .OrderBy(x => x.Day)
                .ToListAsync();
        }

        /// <summary>
        /// Generates recurring bills for the month and year
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="month">The month</param>
        /// <param name="year">The year</param>
        /// <returns></returns>
        public async Task<List<UserBill>> GenerateRecurringBillsAsync(int userId, int month, int year)
        {
            var userBillDetails = await (from ub in _context.UserBills
                                         join bill in _context.Bills on ub.Bill equals bill
                                         join ubrs in _context.UserBillRecurrenceSchedule on ub.RecurrenceSchedule equals ubrs
                                         join rt in _context.RecurrenceType on ubrs.RecurrenceType equals rt
                                         where ub.User.Id == userId && !bill.IsComplete
                                         select new UserBillDetail
                                         {
                                             UserId = userId,
                                             BillId = bill.Id,
                                             Month = month,
                                             Year = year,
                                             RecurrenceTypeName = rt.Name,
                                             RecurrenceTypeId = ubrs.RecurrenceTypeId,
                                             Schedule = ubrs.Schedule,
                                             RecurrenceScheduleId = ub.RecurrenceScheduleId
                                         }).Distinct().ToListAsync();

            foreach (var billDetail in userBillDetails)
            {
                var recModel = GetRecurrenceModel(billDetail.RecurrenceTypeName, billDetail.Schedule);
                await CreateNewUserBillAsync(billDetail, recModel);
            }

            return await GetBillsByUserIdAndMonthYearAsync(userId, month, year);
        }

        /// <summary>
        /// Gets the recurrence model
        /// </summary>
        /// <param name="recurrenceTypeName">The recurrence type</param>
        /// <param name="recurrenceSchedule">The recurrence schedule</param>
        /// <returns></returns>
        private static IRecurrenceModel GetRecurrenceModel(string recurrenceTypeName, string recurrenceSchedule)
        {
            return recurrenceTypeName switch
            {
                "Daily" => (IRecurrenceModel)new DailyRecurrence(),
                "Weekly" => new WeeklyRecurrence(recurrenceSchedule),
                "BiWeeklyOdd" => new BiWeeklyOddRecurrence(recurrenceSchedule),
                "BiWeeklyEven" => new BiWeeklyEvenRecurrence(recurrenceSchedule),
                "BiMonthly" => new BiMonthlyRecurrence(recurrenceSchedule),
                "Monthly" => new MonthlyRecurrence(recurrenceSchedule),
                "Quarterly" => new QuarterlyRecurrence(recurrenceSchedule),
                "BiYearly" => new BiYearlyRecurrence(recurrenceSchedule),
                "Yearly" => new YearlyRecurrence(recurrenceSchedule),
                "OneTime" => new OnetimeRecurrence(recurrenceSchedule),
                _ => new DailyRecurrence()
            };
        }

        /// <summary>
        /// Creates a new user bill
        /// </summary>
        /// <param name="billDetail">The <see cref="UserBillDetail"/></param>
        /// <param name="recModel">The recurrence model</param>
        private async Task CreateNewUserBillAsync(UserBillDetail billDetail, IRecurrenceModel recModel)
        {
            await CreateUserBillsAsync(_context, recModel, billDetail);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Creates a user bill
        /// </summary>
        /// <param name="_context">The database context</param>
        /// <param name="recModel">The recurrence model</param>
        /// <param name="billDetail">The <see cref="UserBillDetail"/></param>
        private static async Task CreateUserBillsAsync(MyBillsContext _context, IRecurrenceModel recModel, UserBillDetail billDetail)
        {
            switch (recModel.Name)
            {
                case "Daily":
                    await CreateDailyRecurrenceUserBillsAsync(_context, billDetail);
                    break;

                case "Weekly":
                    CreateWeeklyRecurrenceUserBills(_context, recModel, billDetail);
                    break;
                case "BiWeeklyOdd":
                    CreateBiWeeklyOddRecurrenceUserBills(_context, recModel, billDetail);
                    break;
                case "BiWeeklyEven":
                    CreateBiWeeklyEvenRecurrenceUserBills(_context, recModel, billDetail);
                    break;
                case "BiMonthly":
                    CreateBiMonthlyRecurrenceUserBills(_context, recModel, billDetail);
                    break;
                case "Monthly":
                    CreateMonthlyRecurrenceUserBills(_context, recModel, billDetail);
                    break;
                case "Quarterly":
                    CreateQuarterlyRecurrenceUserBills(_context, recModel, billDetail);
                    break;
                case "BiYearly":
                    CreateBiYearlyRecurrenceUserBills(_context, recModel, billDetail);
                    break;
                case "Yearly":
                    CreateYearlyRecurrenceUserBills(_context, recModel, billDetail);
                    break;
                case "OneTime":
                    await CreateOneTimeRecurrenceUserBillsAsync(_context, recModel, billDetail);
                    break;
            }
        }

        /// <summary>
        /// Creates a one time bill
        /// </summary>
        /// <param name="_context">The database context</param>
        /// <param name="model">The recurrence model</param>
        /// <param name="billDetail">The bill detail</param>
        private static async Task CreateOneTimeRecurrenceUserBillsAsync(MyBillsContext _context, IRecurrenceModel model, UserBillDetail billDetail)
        {
            var yearlyRecurrenceRec = (OnetimeRecurrence)model;
            var month = yearlyRecurrenceRec.DueDate.Month;
            var year = yearlyRecurrenceRec.DueDate.Year;

            var lastDayOfMonth = DateTime.DaysInMonth(year, month);

            var dueDate = (yearlyRecurrenceRec.DueDate.Day > lastDayOfMonth) ? lastDayOfMonth : yearlyRecurrenceRec.DueDate.Day;

            // Does user bill already exist?
            var userBill = await (from b in _context.Bills
                                  join ub in _context.UserBills on b equals ub.Bill
                                  where ub.UserId == billDetail.UserId && ub.BillId == billDetail.BillId && ub.Day == dueDate
                                   && ub.Month == month && ub.Year == year
                                  select ub).SingleOrDefaultAsync();

            if (userBill != null)
                return;

            var oneTimeUserBill = new UserBill()
            {
                UserId = billDetail.UserId,
                BillId = billDetail.BillId,
                Day = dueDate,
                Month = yearlyRecurrenceRec.DueDate.Month,
                Year = yearlyRecurrenceRec.DueDate.Year,
                RecurrenceScheduleId = billDetail.RecurrenceScheduleId
            };

            _context.UserBills.Add(oneTimeUserBill);
        }

        /// <summary>
        /// Creates a bill with a yearly occurence 
        /// </summary>
        /// <param name="_context">The database context</param>
        /// <param name="model">The recurrence model</param>
        /// <param name="billDetail">The bill detail</param>
        private static void CreateYearlyRecurrenceUserBills(MyBillsContext _context, IRecurrenceModel model, UserBillDetail billDetail)
        {
            var yearlyRecurrenceRec = (YearlyRecurrence)model;
            var lastDayOfMonth = DateTime.DaysInMonth(yearlyRecurrenceRec.DueDate.Year, yearlyRecurrenceRec.DueDate.Month);

            var dueDate = (yearlyRecurrenceRec.DueDate.Day > lastDayOfMonth) ? lastDayOfMonth : yearlyRecurrenceRec.DueDate.Day;

            var yearlyUserBill = new UserBill()
            {
                UserId = billDetail.UserId,
                BillId = billDetail.BillId,
                Day = dueDate,
                Month = yearlyRecurrenceRec.DueDate.Month,
                Year = yearlyRecurrenceRec.DueDate.Year,
                RecurrenceScheduleId = billDetail.RecurrenceScheduleId
            };

            _context.UserBills.Add(yearlyUserBill);
        }

        /// <summary>
        /// Creates a bill with a biyearly occurence 
        /// </summary>
        /// <param name="_context">The database context</param>
        /// <param name="model">The recurrence model</param>
        /// <param name="billDetail">The bill detail</param>
        private static void CreateBiYearlyRecurrenceUserBills(MyBillsContext _context, IRecurrenceModel model, UserBillDetail billDetail)
        {
            var biYearlyRecurrenceRec = (BiYearlyRecurrence)model;
            var lastDayOfFirstMonth = DateTime.DaysInMonth(billDetail.Year, biYearlyRecurrenceRec.FirstMonth);
            var lastDayOfSecondMonth = DateTime.DaysInMonth(billDetail.Year, biYearlyRecurrenceRec.SecondMonth);

            var firstDueDate = (biYearlyRecurrenceRec.FirstDay > lastDayOfFirstMonth) ? lastDayOfFirstMonth : biYearlyRecurrenceRec.FirstDay;
            var secondDueDate = (biYearlyRecurrenceRec.SecondDay > lastDayOfSecondMonth) ? lastDayOfSecondMonth : biYearlyRecurrenceRec.SecondDay;

            var firstBiYearlyUserBill = new UserBill()
            {
                UserId = billDetail.UserId,
                BillId = billDetail.BillId,
                Day = firstDueDate,
                Month = biYearlyRecurrenceRec.FirstMonth,
                Year = billDetail.Year,
                RecurrenceScheduleId = billDetail.RecurrenceScheduleId
            };

            var secondBiYearlyUserBill = new UserBill()
            {
                UserId = billDetail.UserId,
                BillId = billDetail.BillId,
                Day = secondDueDate,
                Month = biYearlyRecurrenceRec.SecondMonth,
                Year = billDetail.Year,
                RecurrenceScheduleId = billDetail.RecurrenceScheduleId
            };

            _context.UserBills.Add(firstBiYearlyUserBill);
            _context.UserBills.Add(secondBiYearlyUserBill);
        }

        /// <summary>
        /// Creates a bill with a quarterly occurence 
        /// </summary>
        /// <param name="_context">The database context</param>
        /// <param name="model">The recurrence model</param>
        /// <param name="billDetail">The bill detail</param>
        private static void CreateQuarterlyRecurrenceUserBills(MyBillsContext _context, IRecurrenceModel model, UserBillDetail billDetail)
        {
            var quarterlyRec = (QuarterlyRecurrence)model;
            var lastDayOfFirstMonth = DateTime.DaysInMonth(billDetail.Year, quarterlyRec.FirstMonth);
            var lastDayOfSecondMonth = DateTime.DaysInMonth(billDetail.Year, quarterlyRec.SecondMonth);
            var lastDayOfThirdMonth = DateTime.DaysInMonth(billDetail.Year, quarterlyRec.ThirdMonth);
            var lastDayOfFourthMonth = DateTime.DaysInMonth(billDetail.Year, quarterlyRec.FourthMonth);

            var firstDueDate = (quarterlyRec.FirstDay > lastDayOfFirstMonth) ? lastDayOfFirstMonth : quarterlyRec.FirstDay;
            var secondDueDate = (quarterlyRec.SecondDay > lastDayOfSecondMonth) ? lastDayOfSecondMonth : quarterlyRec.SecondDay;
            var thirdDueDate = (quarterlyRec.ThirdDay > lastDayOfThirdMonth) ? lastDayOfThirdMonth : quarterlyRec.ThirdDay;
            var fourthDueDate = (quarterlyRec.FourthDay > lastDayOfFourthMonth) ? lastDayOfFourthMonth : quarterlyRec.FourthDay;

            var firstQuarterUserBill = new UserBill()
            {
                UserId = billDetail.UserId,
                BillId = billDetail.BillId,
                Day = firstDueDate,
                Month = quarterlyRec.FirstMonth,
                Year = billDetail.Year,
                RecurrenceScheduleId = billDetail.RecurrenceScheduleId
            };

            var secondQuarterUserBill = new UserBill()
            {
                UserId = billDetail.UserId,
                BillId = billDetail.BillId,
                Day = secondDueDate,
                Month = quarterlyRec.SecondMonth,
                Year = billDetail.Year,
                RecurrenceScheduleId = billDetail.RecurrenceScheduleId
            };

            var thirdQuarterUserBill = new UserBill()
            {
                UserId = billDetail.UserId,
                BillId = billDetail.BillId,
                Day = thirdDueDate,
                Month = quarterlyRec.ThirdMonth,
                Year = billDetail.Year,
                RecurrenceScheduleId = billDetail.RecurrenceScheduleId
            };

            var fourthQuarterUserBill = new UserBill()
            {
                UserId = billDetail.UserId,
                BillId = billDetail.BillId,
                Day = fourthDueDate,
                Month = quarterlyRec.FourthMonth,
                Year = billDetail.Year,
                RecurrenceScheduleId = billDetail.RecurrenceScheduleId
            };

            _context.UserBills.Add(firstQuarterUserBill);
            _context.UserBills.Add(secondQuarterUserBill);
            _context.UserBills.Add(thirdQuarterUserBill);
            _context.UserBills.Add(fourthQuarterUserBill);
        }

        /// <summary>
        /// Creates a bill with a monthly occurence 
        /// </summary>
        /// <param name="_context">The database context</param>
        /// <param name="model">The recurrence model</param>
        /// <param name="billDetail">The bill detail</param>
        private static void CreateMonthlyRecurrenceUserBills(MyBillsContext _context, IRecurrenceModel model, UserBillDetail billDetail)
        {
            for (var i = billDetail.Month; i <= 12; i++)
            {
                var lastDayOfMonth = DateTime.DaysInMonth(billDetail.Year, i);
                var monthlyRec = (MonthlyRecurrence)model;

                var dueDate = (monthlyRec.DueDate > lastDayOfMonth) ? lastDayOfMonth : monthlyRec.DueDate;

                var userBill = new UserBill()
                {
                    UserId = billDetail.UserId,
                    BillId = billDetail.BillId,
                    Day = dueDate,
                    Month = i,
                    Year = billDetail.Year,
                    RecurrenceScheduleId = billDetail.RecurrenceScheduleId
                };

                _context.UserBills.Add(userBill);
            }
        }

        /// <summary>
        /// Creates a bill with a bi-monthly occurence 
        /// </summary>
        /// <param name="_context">The database context</param>
        /// <param name="model">The recurrence model</param>
        /// <param name="billDetail">The bill detail</param>
        private static void CreateBiMonthlyRecurrenceUserBills(MyBillsContext _context, IRecurrenceModel model, UserBillDetail billDetail)
        {
            for (var i = billDetail.Month; i <= 12; i++)
            {
                var lastDayOfMonth = DateTime.DaysInMonth(billDetail.Year, i);
                var biMonthlyRec = (BiMonthlyRecurrence)model;

                var firstDueDate = (biMonthlyRec.FirstDueDate > lastDayOfMonth) ? lastDayOfMonth : biMonthlyRec.FirstDueDate;

                var firstUserBill = new UserBill()
                {
                    UserId = billDetail.UserId,
                    BillId = billDetail.BillId,
                    Day = firstDueDate,
                    Month = i,
                    Year = billDetail.Year,
                    RecurrenceScheduleId = billDetail.RecurrenceScheduleId
                };

                var secondDueDate = (biMonthlyRec.SecondDueDate > lastDayOfMonth) ? lastDayOfMonth : biMonthlyRec.SecondDueDate;

                var secondUserBill = new UserBill()
                {
                    UserId = billDetail.UserId,
                    BillId = billDetail.BillId,
                    Day = secondDueDate,
                    Month = i,
                    Year = billDetail.Year,
                    RecurrenceScheduleId = billDetail.RecurrenceScheduleId
                };

                _context.UserBills.Add(firstUserBill);
                _context.UserBills.Add(secondUserBill);
            }
        }

        /// <summary>
        /// Creates a bill with a weekly occurence 
        /// </summary>
        /// <param name="_context">The database context</param>
        /// <param name="model">The recurrence model</param>
        /// <param name="billDetail">The bill detail</param>
        private static void CreateBiWeeklyEvenRecurrenceUserBills(MyBillsContext _context, IRecurrenceModel model, UserBillDetail billDetail)
        {
            for (var i = billDetail.Month; i <= 12; i++)
            {
                var lastDayOfMonth = DateTime.DaysInMonth(billDetail.Year, i);
                var weeklyRec = (BiWeeklyEvenRecurrence)model;
                var dayOfWeek = weeklyRec.DayOfTheWeek;

                for (var j = 1; j <= lastDayOfMonth; j++)
                {
                    var newDay = new DateTime(billDetail.Year, i, j);
                    var weekOfTheYear = GetIso8601WeekOfYear(newDay);
                    if (dayOfWeek == Cal.GetDayOfWeek(newDay) && IsEven(weekOfTheYear))
                    {
                        var newUserBill = new UserBill()
                        {
                            UserId = billDetail.UserId,
                            BillId = billDetail.BillId,
                            Day = j,
                            Month = i,
                            Year = billDetail.Year,
                            RecurrenceScheduleId = billDetail.RecurrenceScheduleId
                        };

                        _context.UserBills.Add(newUserBill);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a bill with a bi-weekly occurence 
        /// </summary>
        /// <param name="_context">The database context</param>
        /// <param name="model">The recurrence model</param>
        /// <param name="billDetail">The bill detail</param>
        private static void CreateBiWeeklyOddRecurrenceUserBills(MyBillsContext _context, IRecurrenceModel model, UserBillDetail billDetail)
        {
            for (var i = billDetail.Month; i <= 12; i++)
            {
                var lastDayOfMonth = DateTime.DaysInMonth(billDetail.Year, i);
                var weeklyRec = (BiWeeklyOddRecurrence)model;
                var dayOfWeek = weeklyRec.DayOfTheWeek;

                for (var j = 1; j <= lastDayOfMonth; j++)
                {
                    var newDay = new DateTime(billDetail.Year, i, j);
                    var weekOfTheYear = GetIso8601WeekOfYear(newDay);
                    if (dayOfWeek == Cal.GetDayOfWeek(newDay) && !IsEven(weekOfTheYear))
                    {
                        var newUserBill = new UserBill()
                        {
                            UserId = billDetail.UserId,
                            BillId = billDetail.BillId,
                            Day = j,
                            Month = i,
                            Year = billDetail.Year,
                            RecurrenceScheduleId = billDetail.RecurrenceScheduleId
                        };

                        _context.UserBills.Add(newUserBill);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a bill with a weekly occurence 
        /// </summary>
        /// <param name="_context">The database context</param>
        /// <param name="model">The recurrence model</param>
        /// <param name="billDetail">The bill detail</param>
        private static void CreateWeeklyRecurrenceUserBills(MyBillsContext _context, IRecurrenceModel model, UserBillDetail billDetail)
        {
            for (var i = billDetail.Month; i <= 12; i++)
            {
                var lastDayOfMonth = DateTime.DaysInMonth(billDetail.Year, i);
                var weeklyRec = (WeeklyRecurrence)model;
                var dayOfWeek = weeklyRec.DayOfTheWeek;

                for (var j = 1; j <= lastDayOfMonth; j++)
                {
                    var newDay = new DateTime(billDetail.Year, i, j);
                    if (dayOfWeek == Cal.GetDayOfWeek(newDay))
                    {
                        var newUserBill = new UserBill()
                        {
                            UserId = billDetail.UserId,
                            BillId = billDetail.BillId,
                            Day = j,
                            Month = i,
                            Year = billDetail.Year,
                            RecurrenceScheduleId = billDetail.RecurrenceScheduleId
                        };

                        _context.UserBills.Add(newUserBill);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a bill with a daily occurence 
        /// </summary>
        /// <param name="_context">The database context</param>
        /// <param name="model">The recurrence model</param>
        /// <param name="billDetail">The bill detail</param>
        private static async Task CreateDailyRecurrenceUserBillsAsync(MyBillsContext _context, UserBillDetail billDetail)
        {
            for (var i = billDetail.Month; i <= 12; i++)
            {
                var lastDayOfMonth = DateTime.DaysInMonth(billDetail.Year, i);

                var userBill = await (from ub in _context.UserBills
                                      where ub.UserId == billDetail.UserId
                                       && ub.BillId == billDetail.BillId
                                       && ub.Month == i
                                       && ub.Year == billDetail.Year
                                      select ub).ToListAsync();

                for (var j = 1; j <= lastDayOfMonth; j++)
                {
                    if (userBill.Any(x => x.Day == j))
                        continue;

                    var newUserBill = new UserBill()
                    {
                        UserId = billDetail.UserId,
                        BillId = billDetail.BillId,
                        Day = j,
                        Month = i,
                        Year = billDetail.Year,
                        RecurrenceScheduleId = billDetail.RecurrenceScheduleId
                    };

                    _context.UserBills.Add(newUserBill);
                }
            }
        }

        /// <summary>
        /// Gets the ISO8601 Week of the year integer value
        /// </summary>
        /// <param name="time">The datetime</param>
        /// <returns></returns>
        private static int GetIso8601WeekOfYear(DateTime time)
        {
            // From https://stackoverflow.com/questions/11154673/get-the-correct-week-number-of-a-given-date          
            var day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        /// <summary>
        /// Returns a boolean of if the value is even
        /// </summary>
        /// <param name="number">An integer</param>
        /// <returns></returns>
        private static bool IsEven(int number) => number % 2 == 0;
    }
}
