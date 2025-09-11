using PaymentService.Application.DTOs;
using PaymentService.Infrastructure.Data;
using Shared.Core.Entities;
using Shared.Core.Enums;
using System.Security.Claims;

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
                StripeSubscriptionId = dto.PaymentMethodId + dto.PriceId + userId.ToString(),
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

        public int GetCurrentUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("User is not authenticated");

            return userId;
        }
    }
}
