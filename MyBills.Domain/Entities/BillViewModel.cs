using System.Collections.Generic;
using System.ComponentModel;

namespace MyBills.Domain.Entities
{
    public class BillViewModel
    {
        public Bill Bill { get; set; }

        [DisplayName("Recurrence Type")]
        public int RecurrenceTypeId { get; set; }

        public DailyRecurrence DailyRecurrence { get; set; }

        public WeeklyRecurrence WeeklyRecurrence { get; set; }

        public BiWeeklyOddRecurrence BiWeeklyOddRecurrence { get; set; }

        public BiWeeklyEvenRecurrence BiWeeklyEvenRecurrence { get; set; }

        public BiMonthlyRecurrence BiMonthlyRecurrence { get; set; }

        public MonthlyRecurrence MonthlyRecurrence { get; set; }

        public QuarterlyRecurrence QuarterlyRecurrence { get; set; }

        public BiYearlyRecurrence BiYearlyRecurrence { get; set; }

        public YearlyRecurrence YearlyRecurrence { get; set; }

        public OnetimeRecurrence OnetimeRecurrence { get; set; }

        public List<RecurrenceType> RecurrenceTypeList { get; set; }
    }
}
