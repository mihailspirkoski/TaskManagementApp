using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Core.Entities
{
    public class Task
    {
        [Key]
        public int Id { get; set; }
        [Required, StringLength(30)]
        public string Title { get; set; }
        [Required, StringLength(300)]
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

    }
}
