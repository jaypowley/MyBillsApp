using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyBills.Domain.Entities
{
    public sealed class UserDetail
    {
        public int Id { get; set; }

        [DisplayName("First Name"), StringLength(50)]
        public string FirstName { get; set; }
        public byte[] ProfilePicture { get; set; }
        public int UserId { get; set; }

        public User User { get; set; }
    }
}
