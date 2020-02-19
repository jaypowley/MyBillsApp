using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MyBills.Data.Contexts;
using MyBills.Domain.Entities;
using MyBills.Domain.Interfaces;

namespace MyBills.Data.Repositories
{
    public class UserBillRepository: IUserBillRepository
    {
        private static Calendar Cal => CultureInfo.InvariantCulture.Calendar;

        /// <summary>
        /// Marks a bill a paid
        /// </summary>
        /// <param name="billId">The bill id</param>
        /// <param name="userId">The user id</param>
        /// <param name="day">The bill day</param>
        /// <param name="month">The bill month</param>
        /// <param name="year">The bill year</param>
        public void MarkBillAsPaid(int billId, int userId, int day, int month, int year)
        {
            using var ctx = new MyBillsContext();
            var bill = ctx.UserBills.SingleOrDefault(x => x.BillId == billId && x.User.Id == userId && x.Day == day && x.Month == month && x.Year == year);
            if (bill == null) return;

            var newValue = !bill.IsPaid;
            bill.IsPaid = newValue;

            ctx.SaveChanges();
        }

        /// <summary>
        /// Gets the user bill set model by user Id
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <returns></returns>
        public UserBillSet GetBillsByUserIdConsolidated(int userId)
        {
            UserBillSet ubs;
            using (var ctx = new MyBillsContext())
            {

                var userBillDetails = (from ub in ctx.UserBills
                                                        join bill in ctx.Bills on ub.Bill equals bill
                                                        join ubrs in ctx.UserBillRecurrenceSchedule on ub.RecurrenceSchedule equals ubrs
                                                        join rt in ctx.RecurrenceType on ubrs.RecurrenceType equals rt
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
                                                        }
                                                        ).ToList();

                var filteredList = userBillDetails.GroupBy(x => x.BillId)
                                                    .Select(grp => grp.First())
                                                    .ToList();

                ubs = new UserBillSet()
                {
                    UserId = userId,
                    BillDetails = filteredList
                };
            }

            return ubs;
        }

        /// <summary>
        /// Create a new user bill
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="bill">The bill model</param>
        /// <param name="model">The recurrence model</param>
        /// <param name="recurrenceSchedule">The recurrence schedule</param>
        public void CreateNewUserBill(int userId, Bill bill, IRecurrenceModel model, RecurrenceSchedule recurrenceSchedule)
        {
            using var ctx = new MyBillsContext();
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

            CreateUserBills(ctx, model, billDetail);

            ctx.SaveChanges();
        }

        /// <summary>
        /// Gets all bills for the user and month and year
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="month">The month</param>
        /// <param name="year">The year</param>
        /// <returns></returns>
        public List<UserBill> GetBillsByUserIdAndMonthYear(int userId, int month, int year)
        {
            List<UserBill> userBills;
            using (var ctx = new MyBillsContext())
            {
                userBills = ctx.UserBills
                    .Include(x => x.Bill)
                    .Where(x => x.UserId == userId && x.Month == month && x.Year == year && x.Bill.IsComplete == false)
                    .OrderBy(x => x.Day)
                    .ToList();
            }

            return userBills;
        }

        /// <summary>
        /// Generates recurring bills for the month and year
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="month">The month</param>
        /// <param name="year">The year</param>
        /// <returns></returns>
        public List<UserBill> GenerateRecurringBills(int userId, int month, int year)
        {
            List<UserBillDetail> userBillDetails;
            using (var ctx = new MyBillsContext())
            {
                //Get bills by user
                userBillDetails = (from ub in ctx.UserBills
                    join bill in ctx.Bills on ub.Bill equals bill
                    join ubrs in ctx.UserBillRecurrenceSchedule on ub.RecurrenceSchedule equals ubrs
                    join rt in ctx.RecurrenceType on ubrs.RecurrenceType equals rt
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
                    }).Distinct().ToList();
            }

            //Add new user bill with user and bill for the month and year
            foreach (var billDetail in userBillDetails)
            {
                var recModel = GetRecurrenceModel(billDetail.RecurrenceTypeName, billDetail.Schedule);

                CreateNewUserBill(billDetail, recModel);
            }

            var userBills = GetBillsByUserIdAndMonthYear(userId, month, year);
            return userBills;
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
                "Daily" => (IRecurrenceModel) new DailyRecurrence(),
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
        private void CreateNewUserBill(UserBillDetail billDetail, IRecurrenceModel recModel)
        {
            using var ctx = new MyBillsContext();
            CreateUserBills(ctx, recModel, billDetail);
            ctx.SaveChanges();
        }

        /// <summary>
        /// Creates a user bill
        /// </summary>
        /// <param name="ctx">The database context</param>
        /// <param name="recModel">The recurrence model</param>
        /// <param name="billDetail">The <see cref="UserBillDetail"/></param>
        private static void CreateUserBills(MyBillsContext ctx, IRecurrenceModel recModel, UserBillDetail billDetail)
        {
            switch (recModel.Name)
            {
                case "Daily":
                    CreateDailyRecurrenceUserBills(ctx, billDetail);
                    break;

                case "Weekly":
                    CreateWeeklyRecurrenceUserBills(ctx, recModel, billDetail);
                    break;
                case "BiWeeklyOdd":
                    CreateBiWeeklyOddRecurrenceUserBills(ctx, recModel, billDetail);
                    break;
                case "BiWeeklyEven":
                    CreateBiWeeklyEvenRecurrenceUserBills(ctx, recModel, billDetail);
                    break;
                case "BiMonthly":
                    CreateBiMonthlyRecurrenceUserBills(ctx, recModel, billDetail);
                    break;
                case "Monthly":
                    CreateMonthlyRecurrenceUserBills(ctx, recModel, billDetail);
                    break;
                case "Quarterly":
                    CreateQuarterlyRecurrenceUserBills(ctx, recModel, billDetail);
                    break;
                case "BiYearly":
                    CreateBiYearlyRecurrenceUserBills(ctx, recModel, billDetail);
                    break;
                case "Yearly":
                    CreateYearlyRecurrenceUserBills(ctx, recModel, billDetail);
                    break;
                case "OneTime":
                    CreateOneTimeRecurrenceUserBills(ctx, recModel, billDetail);
                    break;
            }
        }
        
        /// <summary>
        /// Creates a one time bill
        /// </summary>
        /// <param name="ctx">The database context</param>
        /// <param name="model">The recurrence model</param>
        /// <param name="billDetail">The bill detail</param>
        private static void CreateOneTimeRecurrenceUserBills(MyBillsContext ctx, IRecurrenceModel model, UserBillDetail billDetail)
        {
            var yearlyRecurrenceRec = (OnetimeRecurrence)model;
            var month = yearlyRecurrenceRec.DueDate.Month;
            var year = yearlyRecurrenceRec.DueDate.Year;

            var lastDayOfMonth = DateTime.DaysInMonth(year, month);

            var dueDate = (yearlyRecurrenceRec.DueDate.Day > lastDayOfMonth) ? lastDayOfMonth : yearlyRecurrenceRec.DueDate.Day;

            // Does user bill already exist?
            var userBill = (from b in ctx.Bills
                                 join ub in ctx.UserBills on b equals ub.Bill
                                 where ub.UserId == billDetail.UserId && ub.BillId == billDetail.BillId && ub.Day == dueDate
                                  && ub.Month == month && ub.Year == year
                                 select ub).SingleOrDefault();

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

            ctx.UserBills.Add(oneTimeUserBill);
        }

        /// <summary>
        /// Creates a bill with a yearly occurence 
        /// </summary>
        /// <param name="ctx">The database context</param>
        /// <param name="model">The recurrence model</param>
        /// <param name="billDetail">The bill detail</param>
        private static void CreateYearlyRecurrenceUserBills(MyBillsContext ctx, IRecurrenceModel model, UserBillDetail billDetail)
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

            ctx.UserBills.Add(yearlyUserBill);
        }

        /// <summary>
        /// Creates a bill with a biyearly occurence 
        /// </summary>
        /// <param name="ctx">The database context</param>
        /// <param name="model">The recurrence model</param>
        /// <param name="billDetail">The bill detail</param>
        private static void CreateBiYearlyRecurrenceUserBills(MyBillsContext ctx, IRecurrenceModel model, UserBillDetail billDetail)
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

            ctx.UserBills.Add(firstBiYearlyUserBill);
            ctx.UserBills.Add(secondBiYearlyUserBill);
        }

        /// <summary>
        /// Creates a bill with a quarterly occurence 
        /// </summary>
        /// <param name="ctx">The database context</param>
        /// <param name="model">The recurrence model</param>
        /// <param name="billDetail">The bill detail</param>
        private static void CreateQuarterlyRecurrenceUserBills(MyBillsContext ctx, IRecurrenceModel model, UserBillDetail billDetail)
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

            ctx.UserBills.Add(firstQuarterUserBill);
            ctx.UserBills.Add(secondQuarterUserBill);
            ctx.UserBills.Add(thirdQuarterUserBill);
            ctx.UserBills.Add(fourthQuarterUserBill);
        }

        /// <summary>
        /// Creates a bill with a monthly occurence 
        /// </summary>
        /// <param name="ctx">The database context</param>
        /// <param name="model">The recurrence model</param>
        /// <param name="billDetail">The bill detail</param>
        private static void CreateMonthlyRecurrenceUserBills(MyBillsContext ctx, IRecurrenceModel model, UserBillDetail billDetail)
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

                ctx.UserBills.Add(userBill);
            }
        }

        /// <summary>
        /// Creates a bill with a bi-monthly occurence 
        /// </summary>
        /// <param name="ctx">The database context</param>
        /// <param name="model">The recurrence model</param>
        /// <param name="billDetail">The bill detail</param>
        private static void CreateBiMonthlyRecurrenceUserBills(MyBillsContext ctx, IRecurrenceModel model, UserBillDetail billDetail)
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

                ctx.UserBills.Add(firstUserBill);
                ctx.UserBills.Add(secondUserBill);
            }
        }

        /// <summary>
        /// Creates a bill with a weekly occurence 
        /// </summary>
        /// <param name="ctx">The database context</param>
        /// <param name="model">The recurrence model</param>
        /// <param name="billDetail">The bill detail</param>
        private static void CreateBiWeeklyEvenRecurrenceUserBills(MyBillsContext ctx, IRecurrenceModel model, UserBillDetail billDetail)
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

                        ctx.UserBills.Add(newUserBill);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a bill with a bi-weekly occurence 
        /// </summary>
        /// <param name="ctx">The database context</param>
        /// <param name="model">The recurrence model</param>
        /// <param name="billDetail">The bill detail</param>
        private static void CreateBiWeeklyOddRecurrenceUserBills(MyBillsContext ctx, IRecurrenceModel model, UserBillDetail billDetail)
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

                        ctx.UserBills.Add(newUserBill);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a bill with a weekly occurence 
        /// </summary>
        /// <param name="ctx">The database context</param>
        /// <param name="model">The recurrence model</param>
        /// <param name="billDetail">The bill detail</param>
        private static void CreateWeeklyRecurrenceUserBills(MyBillsContext ctx, IRecurrenceModel model, UserBillDetail billDetail)
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

                        ctx.UserBills.Add(newUserBill);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a bill with a daily occurence 
        /// </summary>
        /// <param name="ctx">The database context</param>
        /// <param name="model">The recurrence model</param>
        /// <param name="billDetail">The bill detail</param>
        private static void CreateDailyRecurrenceUserBills(MyBillsContext ctx, UserBillDetail billDetail)
        {
            for (var i = billDetail.Month; i <= 12; i++)
            {
                var lastDayOfMonth = DateTime.DaysInMonth(billDetail.Year, i);

                // Does user bill already exist?
                var userBill = (from ub in ctx.UserBills
                                           where ub.UserId == billDetail.UserId
                                            && ub.BillId == billDetail.BillId
                                            && ub.Month == i
                                            && ub.Year == billDetail.Year
                                           select ub).ToList();

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

                    ctx.UserBills.Add(newUserBill);
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
