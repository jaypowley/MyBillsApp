using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using MyBills.Domain.Interfaces;

namespace MyBills.Domain.Entities
{
    public class BiMonthlyRecurrence: IRecurrenceModel
    {
        [Range(1, 31), DisplayName("First Due Date")]
        public int FirstDueDate { get; set; }

        [Range(1, 31), DisplayName("Second Due Date")]
        public int SecondDueDate { get; set; }

        public string Name => "BiMonthly";

        public string Format => JsonSerializer.Serialize(new { type = Name, firstDueDate = FirstDueDate, secondDueDate = SecondDueDate });

        public BiMonthlyRecurrence()
        {
            
        }

        public BiMonthlyRecurrence(string schedule)
        {
            var outObject = JsonSerializer.Deserialize<BiMonthlyRecurrence>(schedule);
            FirstDueDate = outObject.FirstDueDate;
            SecondDueDate = outObject.SecondDueDate;
        }
    }
}
