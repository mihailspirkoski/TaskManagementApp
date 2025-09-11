using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Application;
using PaymentService.Application.DTOs;

namespace PaymentService.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Subscriber, Admin")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionApplicationService _subscriptionService;
        private readonly StripeService _stripeService;
        private readonly TaskServiceClient _taskServiceClient;

        public SubscriptionController(ISubscriptionApplicationService subscriptionService, 
                                       StripeService stripeService, 
                                       TaskServiceClient taskServiceClient) { 
            _subscriptionService = subscriptionService;
            _stripeService = stripeService;
            _taskServiceClient = taskServiceClient;
        }

        [HttpGet("{stripeSubscriptionId}")]
        public async Task<IActionResult> GetSubscription(string stripeSubscriptionId)
        {
            var subscription = await _subscriptionService.GetByStripeIdAsync(stripeSubscriptionId);
            return Ok(subscription);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionDto dto)
        {
            var userId = _subscriptionService.GetCurrentUserId(User);
            var subscription = await _stripeService.CreateSubscriptionAsync(dto, userId, User);
            await _taskServiceClient.UpdateUserRoleAsync(userId, Shared.Core.Enums.UserRole.Subscriber);
            return Ok(subscription);
        }

    }
}
