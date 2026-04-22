using System;
using System.ComponentModel;
using System.Text.Json;
using MyBills.Domain.Interfaces;

namespace MyBills.Domain.Entities
{
    public class BiWeeklyOddRecurrence : IRecurrenceModel
    {
        [DisplayName("Day of the Week")]
        public DayOfWeek DayOfTheWeek { get; set; }

        public string Name => "BiWeeklyOdd";

        public string Format => JsonSerializer.Serialize(new { type = Name, dayOfTheWeek = DayOfTheWeek });

        public BiWeeklyOddRecurrence()
        {

        }

        public BiWeeklyOddRecurrence(string schedule)
        {
            var outObject = JsonSerializer.Deserialize<BiWeeklyOddRecurrence>(schedule);
            DayOfTheWeek = outObject.DayOfTheWeek;
        }
    }
}
