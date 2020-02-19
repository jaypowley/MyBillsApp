using System;
using System.ComponentModel;
using MyBills.Domain.Interfaces;
using Newtonsoft.Json;

namespace MyBills.Domain.Entities
{
    public class BiWeeklyOddRecurrence : IRecurrenceModel
    {
        [DisplayName("Day of the Week")]
        public DayOfWeek DayOfTheWeek { get; set; }

        public string Name => "BiWeeklyOdd";

        public string Format => JsonConvert.SerializeObject(new { type = Name, dayOfTheWeek = DayOfTheWeek });

        public BiWeeklyOddRecurrence()
        {

        }

        public BiWeeklyOddRecurrence(string schedule)
        {
            var outObject = JsonConvert.DeserializeObject<BiWeeklyOddRecurrence>(schedule);
            DayOfTheWeek = outObject.DayOfTheWeek;
        }
    }
}
