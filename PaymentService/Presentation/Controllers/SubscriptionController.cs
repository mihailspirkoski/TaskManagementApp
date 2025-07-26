using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Application;
using PaymentService.Application.DTOs;

namespace PaymentService.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Subscriber, Admin")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionApplicationService _subscriptionService;

        public SubscriptionController(ISubscriptionApplicationService subscriptionService) =>
                    _subscriptionService = subscriptionService;

        [HttpGet("{stripeSubscriptionId}")]
        public async Task<IActionResult> GetSubscription(string stripeSubscriptionId)
        {
            var subscription = await _subscriptionService.GetByStripeIdAsync(stripeSubscriptionId);
            return Ok(subscription);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionDto dto)
        {
            var userId = _subscriptionService.GetCurrentUserId(User);
            var subscription = await _subscriptionService.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetSubscription), new { stripeSubscriptionId = subscription.StripeSubscriptionId }, subscription);
        }

    }
}
