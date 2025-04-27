using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Messaging.Events
{
    public class PaymentLinkGeneratedEvent
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string SubscriptionId { get; set; }
        public int PlanId { get; set; }
        public string CheckoutUrl { get; set; }
    }
}
