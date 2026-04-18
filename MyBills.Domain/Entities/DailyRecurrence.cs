using MyBills.Domain.Interfaces;
using System.Text.Json;

namespace MyBills.Domain.Entities
{
    public class DailyRecurrence: IRecurrenceModel
    {
        public string Name => "Daily";
        public string Format => JsonSerializer.Serialize(new { type = Name, dueDate = "Daily" });
    }
}
