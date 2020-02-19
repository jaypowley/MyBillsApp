using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyBills.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Username { get; set; }

        [Required, StringLength(50)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public IEnumerable<UserDetail> UserDetail { get; set; }
    }
}
