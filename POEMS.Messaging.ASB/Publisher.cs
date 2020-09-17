using POEMS.Messaging.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace POEMS.Messaging.ASB
{
    public class Publisher : BaseQueueActor, IPublisher
    {
        public Publisher(ActionArgs args) : base(args)
        {
        }
        public PublishResult Publish(byte[] message)
        {
            PublishResult pr = new PublishResult();
            try
            {
                var asbMessage = new Message(message);
                queueClient.SendAsync(asbMessage);
            }
            catch (Exception ex)
            {
                pr.Result = false;
            }
            return pr;
        }
    }
}
