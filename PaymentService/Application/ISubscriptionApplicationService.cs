using PaymentService.Application.DTOs;
using Shared.Core.Entities;
using System.Security.Claims;

namespace PaymentService.Application
{
    public interface ISubscriptionApplicationService
    {
        Task<Subscription> CreateAsync(CreateSubscriptionDto dto, int userId);
        Task<Subscription> GetByStripeIdAsync(string stripeSubscriptionId);
        int GetCurrentUserId(ClaimsPrincipal user);
    }
}
