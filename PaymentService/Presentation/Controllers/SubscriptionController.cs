using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Application;
using PaymentService.Application.DTOs;
using System.Security.Claims;

namespace PaymentService.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Subscriber, Admin")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionApplicationService _subscriptionService;
        private readonly IUserApplicationService _userService;

        public SubscriptionController(ISubscriptionApplicationService subscriptionService, IUserApplicationService userApplicationService)
        {
            _subscriptionService = subscriptionService;
            _userService = userApplicationService;
        }
                        

        [HttpGet("{stripeSubscriptionId}")]
        public async Task<IActionResult> GetSubscription(string stripeSubscriptionId)
        {
            var subscription = await _subscriptionService.GetByStripeIdAsync(stripeSubscriptionId);
            return Ok(subscription);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionDto dto)
        {
            var userId = _userService.GetCurrentUserId(User);
            var subscription = await _subscriptionService.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetSubscription), new { stripeSubscriptionId = subscription.StripeSubscriptionId }, subscription);
        }

    }
}
