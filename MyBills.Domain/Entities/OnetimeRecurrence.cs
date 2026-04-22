using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using MyBills.Domain.Interfaces;

namespace MyBills.Domain.Entities
{
    public class OnetimeRecurrence: IRecurrenceModel
    {
        [DisplayName("Due Date")]
        public DateTime DueDate { get; set; }

        public string Name => "Onetime";

        public string Format => JsonSerializer.Serialize(new { type = Name, dueDate = DueDate });

        public OnetimeRecurrence()
        {
            
        }

        public OnetimeRecurrence(string schedule)
        {
            var outObject = JsonSerializer.Deserialize<OnetimeRecurrence>(schedule);
            DueDate = outObject.DueDate;
        }
    }
}
