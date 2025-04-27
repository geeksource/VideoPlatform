using Common.Messaging;
using Common.Messaging.Events;
using Common.Messaging.Queues;
using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Dtos;
using SubscriptionService.Models;
using SubscriptionService.Services;

namespace SubscriptionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly IMessagePublisher _messagePublisher;

        public SubscriptionController(ISubscriptionService subscriptionService, IMessagePublisher messagePublisher)
        {
            _subscriptionService = subscriptionService;
            _messagePublisher = messagePublisher;
        }

        [HttpGet("plans")]
        public async Task<IActionResult> GetPlans()
        {
            var plans = await _subscriptionService.GetPlansAsync();
            return Ok(plans);
        }
        [HttpGet("GetActivePlan")]
        public async Task<IActionResult> GetActivePlan(string UserId)
        {
            var plans = await _subscriptionService.GetActivePlan(UserId);
            return Ok(plans);
        }
        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe(SubscribeRequestDto dto)
        {
            var plan = await _subscriptionService.GetPlansAsync(dto.PlanId);
            dto.ReferenceId=Guid.NewGuid().ToString();
            var success = await _subscriptionService.SubscribeAsync(dto);
            if (!success) return BadRequest("Invalid plan.");

            await _messagePublisher.PublishAsync(new SubscriptionRequestedEvent
            {
                Amount = (long)plan.Price,
                UserId = dto.UserId,
                PlanId = dto.PlanId,
                PlanName = plan.Name,
                UserEmail = dto.UserEmail,
                ReferenceId = dto.ReferenceId
            },RabbitMqQueues.Subscription_Requested);

            return Ok(new
            {
                Message = "Subscription request initiated. Awaiting payment confirmation.",
                ReferenceId = dto.ReferenceId
            });
        }
    }
}
