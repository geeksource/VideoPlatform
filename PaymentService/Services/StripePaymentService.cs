using Common.Messaging.Events;
using PaymentService.Models;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using Stripe.Checkout;
using Stripe;
using Microsoft.Extensions.Options;

namespace PaymentService.Services
{
    public class StripePaymentService : IPaymentService
    {
        private readonly IConfiguration _config;
        private readonly StripeSettings _settings;

        public StripePaymentService(IConfiguration config, IOptions<StripeSettings> stripeOptions)
        {
            _config = config;
            _settings = stripeOptions.Value;
        }
        public async Task<string> CreateCheckoutSessionAsync(SubscriptionRequestedEvent subEvent)
        {
            StripeConfiguration.ApiKey = _settings.SecretKey;
            long AmountCents = subEvent.Amount * 100;
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new()
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "usd",
                            UnitAmount = AmountCents,
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = subEvent.PlanName
                            }
                        },
                        Quantity = 1,
                       
                    }
                },
                Metadata = new Dictionary<string, string>
                {
                    { "PlanName", subEvent.PlanName },
                    { "Amount", AmountCents.ToString() },
                    { "PlanId",subEvent.PlanId.ToString()},
                    { "ReferenceId",subEvent.ReferenceId}
                },
                Mode = "payment",
                CustomerEmail = subEvent.UserEmail,
                SuccessUrl = _settings.SuccessUrl,// _configuration["Stripe:SuccessUrl"],
                CancelUrl = _settings.CancelUrl,// _configuration["Stripe:CancelUrl"],
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);
           
            return session.Url;
        }
    }

}
