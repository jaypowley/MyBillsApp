using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MyBills.Domain.Interfaces;
using Newtonsoft.Json;

namespace MyBills.Domain.Entities
{
    public class MonthlyRecurrence: IRecurrenceModel
    {
        [Range(1, 31), DisplayName("Due Date")]
        public int DueDate { get; set; }

        public string Name => "Monthly";

        public string Format => JsonConvert.SerializeObject(new { type = Name, dueDate = DueDate });

        public MonthlyRecurrence()
        {
            
        }

        public MonthlyRecurrence(string schedule)
        {
            var outObject = JsonConvert.DeserializeObject<MonthlyRecurrence>(schedule);
            DueDate = outObject.DueDate;
        }
    }
}
