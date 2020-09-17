using System;
using System.Collections.Generic;
using System.Text;

namespace POEMS.Messaging.RMQ
{
    public class ActionArgs
    {
        public string Queue { get; set; }
        public bool Durable { get; set; }
        public bool Exclusive { get; set; }
        public bool AutoDelete { get; set; }
    }
}
