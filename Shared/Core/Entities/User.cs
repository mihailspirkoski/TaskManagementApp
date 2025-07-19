using Shared.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Core.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required, StringLength(256)]
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public ICollection<Task> Tasks { get; set; } = new List<Task>();

    }
}
