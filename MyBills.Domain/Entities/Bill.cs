using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBills.Domain.Entities
{
    public class Bill
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }

        [DataType(DataType.Currency), Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [DisplayName("Is Complete?")]
        public bool IsComplete { get; set; }

        [DisplayName("Is Auto Paid?")]
        public bool IsAutoPaid { get; set; }
    }
}
