using System;
using System.ComponentModel;
using MyBills.Domain.Interfaces;
using Newtonsoft.Json;

namespace MyBills.Domain.Entities
{
    public class WeeklyRecurrence: IRecurrenceModel
    {
        [DisplayName("Day of the Week")]
        public DayOfWeek DayOfTheWeek { get; set; }

        public string Name => "Weekly";

        public string Format => JsonConvert.SerializeObject(new { type = Name, dayOfTheWeek = DayOfTheWeek });

        public WeeklyRecurrence()
        {

        }

        public WeeklyRecurrence(string schedule)
        {
            var outObject = JsonConvert.DeserializeObject<WeeklyRecurrence>(schedule);
            DayOfTheWeek = outObject.DayOfTheWeek;
        }
    }
}
