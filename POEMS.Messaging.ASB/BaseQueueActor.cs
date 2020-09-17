using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.Amqp;

namespace POEMS.Messaging.ASB
{
    public abstract class BaseQueueActor
    {
        protected ActionArgs _args;
        protected QueueClient queueClient;
        public BaseQueueActor (ActionArgs args)
        {
            _args = args;
            queueClient = new QueueClient(_args.ConnectionString, _args.Queue);
        }
    }
}
