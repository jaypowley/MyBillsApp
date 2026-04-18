using System;
using System.ComponentModel;
using System.Text.Json;
using MyBills.Domain.Interfaces;

namespace MyBills.Domain.Entities
{
    public class YearlyRecurrence: IRecurrenceModel
    {
        [DisplayName("Due Date")]
        public DateTime DueDate { get; set; }

        public string Name => "Yearly";

        public string Format => JsonSerializer.Serialize(new { type = Name, dueDate = DueDate });

        public YearlyRecurrence()
        {
        
        }

        public YearlyRecurrence(string schedule)
        {
            var outObject = JsonSerializer.Deserialize<YearlyRecurrence>(schedule);
            DueDate = outObject.DueDate;
        }
    }
}
