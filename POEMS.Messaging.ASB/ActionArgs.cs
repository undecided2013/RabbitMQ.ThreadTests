using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.ServiceBus;

namespace POEMS.Messaging.ASB
{
    public class ActionArgs
    {
        public string Queue { get; set; }
        public string ConnectionString { get; set; }
        public bool AutoComplete { get; set; }
        public int MaxConcurrentCalls { get; set; }
    }
}
