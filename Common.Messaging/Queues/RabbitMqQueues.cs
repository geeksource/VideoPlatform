using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Messaging.Queues
{
    public static class RabbitMqQueues
    {
        public const string User_Registered = "user-registered";
        public const string Subscription_Requested = "subscription-requested";
        public const string Payment_Link_Generated = "payment-link-generated";
        public const string Payment_Completed = "payment-completed";
    }
}
