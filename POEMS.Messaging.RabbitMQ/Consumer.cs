using POEMS.Messaging.Abstractions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace POEMS.Messaging.RMQ
{
    public class Consumer : BaseQueueActor, IConsumer
    {
        private EventingBasicConsumer basicConsumer;
        
        public Consumer (IConnection connection, ActionArgs args): base(connection, args)
        {
            basicConsumer = new EventingBasicConsumer(_channel);
        }
        public void ReceiveAsync(Func<byte[], bool> processorMethod)
        {
            basicConsumer.Received += (model, ea) =>
            {
                processorMethod(ea.Body.ToArray());
            };
            _channel.BasicConsume( queue: _args.Queue, autoAck: true, consumer: basicConsumer);
        }
    }
}
