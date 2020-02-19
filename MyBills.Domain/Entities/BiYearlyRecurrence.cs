using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MyBills.Domain.Interfaces;
using Newtonsoft.Json;

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

        public string Format => JsonConvert.SerializeObject(new
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
            var outObject = JsonConvert.DeserializeObject<BiYearlyRecurrence>(schedule);
            FirstMonth = outObject.FirstMonth;
            FirstDay = outObject.FirstDay;
            SecondMonth = outObject.SecondMonth;
            SecondDay = outObject.SecondDay;
        }
    }
}
