using Shared.Core.Enums;

namespace PaymentService.Application.DTOs
{
    public class SubscriptionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string StripeSubscriptionId { get; set; }
        public SubscriptionStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
