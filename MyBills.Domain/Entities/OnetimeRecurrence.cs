using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MyBills.Domain.Interfaces;
using Newtonsoft.Json;

namespace MyBills.Domain.Entities
{
    public class OnetimeRecurrence: IRecurrenceModel
    {
        [DisplayName("Due Date")]
        public DateTime DueDate { get; set; }

        public string Name => "Onetime";

        public string Format => JsonConvert.SerializeObject(new { type = Name, dueDate = DueDate });

        public OnetimeRecurrence()
        {
            
        }

        public OnetimeRecurrence(string schedule)
        {
            var outObject = JsonConvert.DeserializeObject<OnetimeRecurrence>(schedule);
            DueDate = outObject.DueDate;
        }
    }
}
