using Shared.Core.Entities;

namespace PaymentService.Infrastructure.Data
{
    public interface ISubscriptionRepository
    {
        Task<Subscription> GetByIdAsync(int id);
        Task<Subscription> GetByStripeIdAsync(string stripeSubscriptionId);
        Task<IEnumerable<Subscription>> GetByUserIdAsync(int userId);
        System.Threading.Tasks.Task AddAsync(Subscription subscription);
        System.Threading.Tasks.Task UpdateAsync(Subscription subscription);
    }
}
