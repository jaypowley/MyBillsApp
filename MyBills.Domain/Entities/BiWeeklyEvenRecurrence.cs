using System;
using System.ComponentModel;
using System.Text.Json;
using MyBills.Domain.Interfaces;

namespace MyBills.Domain.Entities
{
    public class BiWeeklyEvenRecurrence : IRecurrenceModel
    {
        [DisplayName("Day of the Week")]
        public DayOfWeek DayOfTheWeek { get; set; }

        public string Name => "BiWeeklyEven";

        public string Format => JsonSerializer.Serialize(new { type = Name, dayOfTheWeek = DayOfTheWeek });

        public BiWeeklyEvenRecurrence()
        {
            
        }

        public BiWeeklyEvenRecurrence(string schedule)
        {
            var outObject = JsonSerializer.Deserialize<BiWeeklyEvenRecurrence>(schedule);
            DayOfTheWeek = outObject.DayOfTheWeek;
        }
    }
}
