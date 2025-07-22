using Microsoft.EntityFrameworkCore;
using Shared.Core.Entities;

namespace PaymentService.Infrastructure.Data
{
    public class SubscriptionRepository : ISubscriptionRepository
    {

        private readonly PaymentDbContext _context;

        public SubscriptionRepository(PaymentDbContext context) => _context = context;

        public async System.Threading.Tasks.Task AddAsync(Subscription subscription)
        {
            await _context.Subscriptions.AddAsync(subscription);
            await _context.SaveChangesAsync();
        }

        public async Task<Subscription> GetByIdAsync(int id) => await _context.Subscriptions.FindAsync(id) ?? 
            throw new KeyNotFoundException($"Subscription with ID {id} not found.");


        public async Task<Subscription> GetByStripeIdAsync(string stripeSubscriptionId) => await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.StripeSubscriptionId == stripeSubscriptionId) ??
            throw new KeyNotFoundException($"Subscription with Stripe ID {stripeSubscriptionId} not found.");

        public async Task<IEnumerable<Subscription>> GetByUserIdAsync(int userId) => await _context.Subscriptions
            .Where(s => s.UserId == userId)
            .ToListAsync();

        public async System.Threading.Tasks.Task UpdateAsync(Subscription subscription)
        {
            _context.Subscriptions.Update(subscription);
            await _context.SaveChangesAsync();
        }
    }
}
