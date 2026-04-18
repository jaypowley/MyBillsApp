using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using MyBills.Domain.Interfaces;

namespace MyBills.Domain.Entities
{
    public class BiYearlyRecurrence: IRecurrenceModel
    {
        [Range(1, 12), DisplayName("First Month")]
        public int FirstMonth { get; set; }

        [Range(1, 31), DisplayName("First Day")]
        public int FirstDay { get; set; }

        [Range(1, 12), DisplayName("Second Month")]
        public int SecondMonth { get; set; }

        [Range(1, 31), DisplayName("Second Day")]
        public int SecondDay { get; set; }

        public string Name => "BiYearly";

        public string Format => JsonSerializer.Serialize(new
        {
            type = Name,
            firstMonth = FirstMonth,
            firstDay = FirstDay,
            secondMonth = SecondMonth,
            secondDay = SecondDay
        });

        public BiYearlyRecurrence()
        {
            
        }

        public BiYearlyRecurrence(string schedule)
        {
            var outObject = JsonSerializer.Deserialize<BiYearlyRecurrence>(schedule);
            FirstMonth = outObject.FirstMonth;
            FirstDay = outObject.FirstDay;
            SecondMonth = outObject.SecondMonth;
            SecondDay = outObject.SecondDay;
        }
    }
}
