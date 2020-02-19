using MyBills.Domain.Interfaces;
using Newtonsoft.Json;

namespace MyBills.Domain.Entities
{
    public class DailyRecurrence: IRecurrenceModel
    {
        public string Name => "Daily";
        public string Format => JsonConvert.SerializeObject(new { type = Name, dueDate = "Daily" });
    }
}
