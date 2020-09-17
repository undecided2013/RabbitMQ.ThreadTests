using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace POEMS.Messaging.RMQ
{
    public abstract class BaseQueueActor
    {
        protected IConnection _connection;
        protected IModel _channel;
        protected bool isQueueDeclared = false;
        protected ActionArgs _args;
        public BaseQueueActor (IConnection connection, ActionArgs args)
        {
            _connection = connection;
            _channel = _connection.CreateModel();
            _args = args;
            _channel.QueueDeclare(queue: _args.Queue, durable: _args.Durable, exclusive: _args.Exclusive, autoDelete: _args.AutoDelete, arguments: null);
        }
    }
}
