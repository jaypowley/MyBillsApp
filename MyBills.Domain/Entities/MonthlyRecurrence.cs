using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using MyBills.Domain.Interfaces;

namespace MyBills.Domain.Entities
{
    public class MonthlyRecurrence: IRecurrenceModel
    {
        [Range(1, 31), DisplayName("Due Date")]
        public int DueDate { get; set; }

        public string Name => "Monthly";

        public string Format => JsonSerializer.Serialize(new { type = Name, dueDate = DueDate });

        public MonthlyRecurrence()
        {
            
        }

        public MonthlyRecurrence(string schedule)
        {
            var outObject = JsonSerializer.Deserialize<MonthlyRecurrence>(schedule);
            DueDate = outObject.DueDate;
        }
    }
}
