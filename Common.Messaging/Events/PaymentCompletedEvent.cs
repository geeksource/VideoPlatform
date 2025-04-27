using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Messaging.Events
{
    public class PaymentCompletedEvent
    {
        public string Email { get; set; }
        public string PlanName { get; set; }
        public long Amount { get; set; }
        public string SessionId { get; set; }
        public string ReferenceId { get; set; }
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    }

}
