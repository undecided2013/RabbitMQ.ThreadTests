using POEMS.Messaging.Abstractions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace POEMS.Messaging.RMQ
{
    public class Publisher : BaseQueueActor, IPublisher
    {
        public Publisher(IConnection connection, ActionArgs args) : base(connection, args)
        {
        }
        public PublishResult Publish(byte[] message)
        {
            PublishResult pr = new PublishResult();
            try
            {
                _channel.BasicPublish(exchange: "", routingKey: _args.Queue, basicProperties: null, body: message);
                pr.Result = true;
            }
            catch (Exception ex)
            {
                pr.Result = false;
            }
            return pr;
        }
    }
}
