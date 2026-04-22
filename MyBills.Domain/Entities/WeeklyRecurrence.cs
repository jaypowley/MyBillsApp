using System;
using System.ComponentModel;
using System.Text.Json;
using MyBills.Domain.Interfaces;

namespace MyBills.Domain.Entities
{
    public class WeeklyRecurrence: IRecurrenceModel
    {
        [DisplayName("Day of the Week")]
        public DayOfWeek DayOfTheWeek { get; set; }

        public string Name => "Weekly";

        public string Format => JsonSerializer.Serialize(new { type = Name, dayOfTheWeek = DayOfTheWeek });

        public WeeklyRecurrence()
        {

        }

        public WeeklyRecurrence(string schedule)
        {
            var outObject = JsonSerializer.Deserialize<WeeklyRecurrence>(schedule);
            DayOfTheWeek = outObject.DayOfTheWeek;
        }
    }
}
