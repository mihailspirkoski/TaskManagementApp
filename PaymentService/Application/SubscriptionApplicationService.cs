using PaymentService.Application.DTOs;
using PaymentService.Infrastructure.Data;
using Shared.Core.Entities;
using Shared.Core.Enums;

namespace PaymentService.Application
{
    public class SubscriptionApplicationService : ISubscriptionApplicationService
    {

        private readonly ISubscriptionRepository _subscriptionRepository;
        public SubscriptionApplicationService(ISubscriptionRepository subscriptionRepository)
            => _subscriptionRepository = subscriptionRepository;

        public async Task<Subscription> CreateAsync(CreateSubscriptionDto dto, int userId)
        {
            var subscription = new Subscription
            {
                StripeSubscriptionId = dto.StripeSubscriptionId,
                UserId = userId,
                Status = SubscriptionStatus.Active,
                CreatedAt = DateTime.UtcNow
            };
            await _subscriptionRepository.AddAsync(subscription);
            return subscription;
        }

        public async Task<Subscription> GetByStripeIdAsync(string stripeSubscriptionId)
                => await _subscriptionRepository.GetByStripeIdAsync(stripeSubscriptionId) 
                   ?? throw new KeyNotFoundException($"Subscription with Stripe ID {stripeSubscriptionId} not found.");
    }
}
