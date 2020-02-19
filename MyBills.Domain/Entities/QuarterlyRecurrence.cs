using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MyBills.Domain.Interfaces;
using Newtonsoft.Json;

namespace MyBills.Domain.Entities
{
    public class QuarterlyRecurrence: IRecurrenceModel
    {
        [Range(1, 12), DisplayName("First Month")]
        public int FirstMonth { get; set; }

        [Range(1, 31), DisplayName("First Day")]
        public int FirstDay { get; set; }

        [Range(1, 12), DisplayName("Second Month")]
        public int SecondMonth { get; set; }

        [Range(1, 31), DisplayName("Second Day")]
        public int SecondDay { get; set; }

        [Range(1, 12), DisplayName("Third Month")]
        public int ThirdMonth { get; set; }

        [Range(1, 31), DisplayName("Third Day")]
        public int ThirdDay { get; set; }

        [Range(1, 12), DisplayName("Fourth Month")]
        public int FourthMonth { get; set; }

        [Range(1, 31), DisplayName("Fourth Day")]
        public int FourthDay { get; set; }

        public string Name => "Quarterly";

        public string Format => JsonConvert.SerializeObject(new
        {
            type = Name,
            firstMonth = FirstMonth,
            firstDay = FirstDay,
            secondMonth = SecondMonth,
            secondDay = SecondDay,
            thirdMonth = ThirdMonth,
            thirdDay = ThirdDay,
            fourthMonth = FourthMonth,
            fourthDay = FourthDay
        });

        public QuarterlyRecurrence()
        {
            
        }

        public QuarterlyRecurrence(string schedule)
        {
            if (string.IsNullOrWhiteSpace(schedule))
            {
                throw new NullReferenceException();
            }

            var outObject = JsonConvert.DeserializeObject<QuarterlyRecurrence>(schedule);
            FirstMonth = outObject.FirstMonth;
            FirstDay = outObject.FirstDay;
            SecondMonth = outObject.SecondMonth;
            SecondDay = outObject.SecondDay;
            ThirdMonth = outObject.ThirdMonth;
            ThirdDay = outObject.ThirdDay;
            FourthMonth = outObject.FourthMonth;
            FourthDay = outObject.FourthDay;
        }
    }
}
