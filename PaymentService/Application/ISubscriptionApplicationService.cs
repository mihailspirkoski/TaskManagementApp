using PaymentService.Application.DTOs;
using Shared.Core.Entities;

namespace PaymentService.Application
{
    public interface ISubscriptionApplicationService
    {
        Task<Subscription> CreateAsync(CreateSubscriptionDto dto, int userId);
        Task<Subscription> GetByStripeIdAsync(string stripeSubscriptionId);
    }
}
