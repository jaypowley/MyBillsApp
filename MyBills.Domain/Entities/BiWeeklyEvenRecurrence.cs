using System;
using System.ComponentModel;
using MyBills.Domain.Interfaces;
using Newtonsoft.Json;

namespace MyBills.Domain.Entities
{
    public class BiWeeklyEvenRecurrence : IRecurrenceModel
    {
        [DisplayName("Day of the Week")]
        public DayOfWeek DayOfTheWeek { get; set; }

        public string Name => "BiWeeklyEven";

        public string Format => JsonConvert.SerializeObject(new { type = Name, dayOfTheWeek = DayOfTheWeek });

        public BiWeeklyEvenRecurrence()
        {
            
        }

        public BiWeeklyEvenRecurrence(string schedule)
        {
            var outObject = JsonConvert.DeserializeObject<BiWeeklyEvenRecurrence>(schedule);
            DayOfTheWeek = outObject.DayOfTheWeek;
        }
    }
}
