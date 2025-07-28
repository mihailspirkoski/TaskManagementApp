using Microsoft.EntityFrameworkCore;
using Shared.Core.Entities;

namespace PaymentService.Infrastructure.Data
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
        {
        }

        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            SeedData.Seed(modelBuilder);
            // Configure Subscription entity
            modelBuilder.Entity<Subscription>()
                .Property(s => s.Status)
                .HasConversion<string>();
            modelBuilder.Entity<Subscription>()
                .Property(s => s.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}
