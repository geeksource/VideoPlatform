using Common.Messaging.Events;

namespace PaymentService.Services
{
    public interface IPaymentService
    {
        Task<string> CreateCheckoutSessionAsync(SubscriptionRequestedEvent subEvent);
    }
}
