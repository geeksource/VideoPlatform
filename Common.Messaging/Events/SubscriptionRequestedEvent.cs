using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Messaging.Events
{
    public class SubscriptionRequestedEvent
    {
        public string UserId { get; set; } = string.Empty;
        public int PlanId { get; set; }
        public string PlanName { get; set; }
        public long Amount { get; set; }
        public string ReferenceId { get; set; }
        public string UserEmail { get; set; }

    }
}
