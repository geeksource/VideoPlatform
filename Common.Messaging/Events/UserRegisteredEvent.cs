using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Messaging.Events
{
    public class UserRegisteredEvent
    {
        public string Email { get; set; }
        public string ConfirmationLink { get; set; }
    }
}
