using PaymentService.Application.DTOs;
using PaymentService.Infrastructure.Data;
using Stripe;
using System.Security.Claims;

namespace PaymentService.Application
{
    public class StripeService
    {
        private readonly string _stripeSecretKey;
        private ISubscriptionRepository _subscriptionRepository;

        public StripeService(IConfiguration configuration, ISubscriptionRepository subscriptionRepository)
        {
            _stripeSecretKey = configuration["Stripe:SecretKey"];
            _subscriptionRepository = subscriptionRepository;
            Stripe.StripeConfiguration.ApiKey = _stripeSecretKey;
        }
        // Methods to interact with Stripe API would go here
        public async Task<SubscriptionDto>CreateSubscriptionAsync(CreateSubscriptionDto dto, int userId, ClaimsPrincipal user)
        {
            try
            {
                //Create or retrieve a customer in Stripe
                var customerService = new Stripe.CustomerService();
                var customer = await customerService.CreateAsync(new Stripe.CustomerCreateOptions
                {
                    PaymentMethod = dto.PaymentMethodId,
                    Email = user.FindFirst(ClaimTypes.Email)?.Value,
                    InvoiceSettings = new Stripe.CustomerInvoiceSettingsOptions
                    {
                        DefaultPaymentMethod = dto.PaymentMethodId,
                    }
                });

                var subscriptionService = new Stripe.SubscriptionService();
                var stripeSubscription = await subscriptionService.CreateAsync(new Stripe.SubscriptionCreateOptions
                {
                    Customer = customer.Id,
                    Items = new List<Stripe.SubscriptionItemOptions>
                    {
                        new Stripe.SubscriptionItemOptions
                        {
                            Price = dto.PriceId,
                        },
                    },
                    PaymentBehavior = "default_incomplete",
                    PaymentSettings = new SubscriptionPaymentSettingsOptions
                    {
                        PaymentMethodTypes = new List<string> { "card" },
                        SaveDefaultPaymentMethod = "on_subscription"
                    }
                });

                var subscripion = new Shared.Core.Entities.Subscription
                {
                    UserId = userId,
                    StripeSubscriptionId = stripeSubscription.Id,
                    Status = Shared.Core.Enums.SubscriptionStatus.Active,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                
                await _subscriptionRepository.AddAsync(subscripion);

                return new SubscriptionDto
                {
                    Id = subscripion.Id,
                    UserId = subscripion.UserId,
                    StripeSubscriptionId = subscripion.StripeSubscriptionId,
                    Status = subscripion.Status,
                    CreatedAt = subscripion.CreatedAt,
                    UpdatedAt = subscripion.UpdatedAt ?? DateTime.UtcNow
                };
            }
            catch(StripeException ex)
            {
                throw new Exception($"Stripe error: {ex.Message}");
            }
        }
    }
}
