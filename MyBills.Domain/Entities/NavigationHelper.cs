using System;

namespace MyBills.Domain.Entities
{
    public class NavigationHelper
    {
        public DateTime Today => DateTime.Now;
        public int CurrentMonth;
        public int CurrentYear;
        public int PrevMonth;
        public int NextMonth;
        public int PrevMonthsYear;
        public int NextMonthsYear;
    }
}
