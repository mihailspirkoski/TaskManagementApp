using Microsoft.EntityFrameworkCore;
using Shared.Core.Entities;
using Shared.Core.Enums;

namespace PaymentService.Infrastructure.Data
{
    public class SeedData
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subscription>().HasData(
                new Subscription
                {
                    Id = 1,
                    UserId = 2, 
                    StripeSubscriptionId = "sub_123456789",
                    Status = SubscriptionStatus.Active,
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
