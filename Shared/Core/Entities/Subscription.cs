using Shared.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Core.Entities
{
    public class Subscription
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        [Required, StringLength(100)]
        public string StripeSubscriptionId { get; set; }
        public SubscriptionStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
