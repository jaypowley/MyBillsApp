using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyBills.Domain.Entities
{
    public class UserRegistration
    {
        public string Username { get; set; }

        [Required, StringLength(50)]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required, DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Required, DisplayName("Friendly Name"), StringLength(50)]
        public string FriendlyName { get; set; }

        public bool IsSuccess { get; set; }
    }
}
