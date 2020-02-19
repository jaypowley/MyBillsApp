using System.ComponentModel.DataAnnotations;

namespace MyBills.Domain.Entities
{
    public class RecurrenceType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        //[Required] - Daily is null
        public string Type { get; set; }
    }
}
