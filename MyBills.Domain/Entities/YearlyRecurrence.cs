using System;
using System.ComponentModel;
using MyBills.Domain.Interfaces;
using Newtonsoft.Json;

namespace MyBills.Domain.Entities
{
    public class YearlyRecurrence: IRecurrenceModel
    {
        [DisplayName("Due Date")]
        public DateTime DueDate { get; set; }

        public string Name => "Yearly";

        public string Format => JsonConvert.SerializeObject(new { type = Name, dueDate = DueDate });

        public YearlyRecurrence()
        {
        
        }

        public YearlyRecurrence(string schedule)
        {
            var outObject = JsonConvert.DeserializeObject<YearlyRecurrence>(schedule);
            DueDate = outObject.DueDate;
        }
    }
}
