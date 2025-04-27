using Common.Messaging;
using Common.Messaging.Events;
using Common.Messaging.Queues;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StripeWebhookController : ControllerBase
    {
        private readonly ILogger<StripeWebhookController> _logger;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IConfiguration _configuration;

        public StripeWebhookController(ILogger<StripeWebhookController> logger,IMessagePublisher messagePublisher,IConfiguration configuration)
        {
            _logger = logger;
            _messagePublisher = messagePublisher;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _configuration.GetSection("Stripe")["WebHookUrl"] //"whsec_YOUR_WEBHOOK_SECRET" // Get this from Stripe dashboard
                );

                if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
                {
                    var session = stripeEvent.Data.Object as Session;

                    // ✅ Handle successful payment
                    _logger.LogInformation($"Payment completed for session: {session?.Id}");

                    // 🔔 Optionally publish a message to RabbitMQ
                    // e.g. _messagePublisher.PublishAsync(new PaymentCompletedEvent { ... });
                    var completedEvent = new PaymentCompletedEvent
                    {
                        Email = session.CustomerEmail,
                        PlanName = session.Metadata["PlanName"],
                        Amount = long.Parse(session.Metadata["Amount"]),
                        SessionId = session.Id,
                        ReferenceId = session.Metadata["ReferenceId"]
                    };

                    await _messagePublisher.PublishAsync(completedEvent, RabbitMqQueues.Payment_Completed);

                    return Ok();
                }

                return Ok();
            }
            catch (StripeException e)
            {
                _logger.LogError($"Stripe webhook error: {e.Message}");
                return BadRequest();
            }
        }

        [HttpGet]
        public ActionResult Get()
        {
            return Ok("Working Good");
        }
    }

}
