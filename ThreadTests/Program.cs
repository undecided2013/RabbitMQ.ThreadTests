using Microsoft.Extensions.DependencyInjection;
using POEMS.Messaging.Abstractions;
using POEMS.Messaging.RMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace ThreadTests
{
    /*
    class QueuePublisher
    {
        IConnection _connection;
        IModel channel;
        int _target;
        // When an instance is created, the target number needs to be specified
        public QueuePublisher(int target, IConnection connection)
        {
            // The targer number is then stored in the class private variable _target
            this._target = target;
            _connection = connection;
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }
        // Function prints the numbers from 1 to the traget number that the user provided
        public void Publish()
        {
            var tickStart = Environment.TickCount64;
            string message = "sdjhflsdkjhflsadkfhsldjfhsdlkjfhsdlkjfhsdlkjfhsdlkjfhsdlkjfhsdlkfjhdlkjgfhdljghdlghsdlfkjghdlkgjhdflkjghdlkjghdljghdlfkjghdlskjghdlkjghdlfkjghdlkjghdlkfjghdslfkjghdslkfjghdlskjghdlkgjhdflkjghdlfkjghdlkfjghdflkgjhdsflkjghdlfkgjhdlfjghdslkgjhdslgjhdslgjhsdlgjhdslkgjhdslkgjhdlkgjhdlfjghdlfkjghdlkfjghdlfgjhdlfkjghldfjghlsdfjghdlfgjhdslkfgjhdlkfjghdslkfhgdlkfsjghdslkfjghldksfjghdlfkgjhdlskfgjhdlkgjhdslkjghdslkjghldskjghlsdkgjhsdlkghsdlkghdslkfghjdlfjghdlfjghldfjghldkhglksdjhgfsdljhgsldkgjhdslkgjhdslfkjghdslkfgjhsdlkgjhsdlkfgjhsdlfkjghdslfkjghsdlkgjhdslkfjghdslkfjghdslkfjghdsklfjghdslfkjghdflkjghsldkgjhdslkfgjhsdlkgjhdlkfgjhdlgkjhdlkfgjhdflkgjhdflkgjhdlkgjhdglkjhglkdfsjhgdlfkjghldskjghdslkjgfhsdklfjghdlkfjghdlfkjghslkgjhdlfkjghsdlkfjgh";
            var body = Encoding.UTF8.GetBytes(message);
            for (int x = 0; x <= _target; x++)
            {
                channel.BasicPublish(exchange: "", routingKey: "hello", basicProperties: null, body: body);
            }
            var tickEnd = Environment.TickCount64;
            Console.WriteLine($"Published {_target} messages in {tickEnd-tickStart} ms");
        }
    }


    class QueueConsumer
    {
        IConnection _target;
        IModel channel;
        CountdownEvent _countdown;
        // When an instance is created, the target number needs to be specified
        public QueueConsumer(IConnection connection, CountdownEvent countdown)
        {
            // The targer number is then stored in the class private variable _target
            this._target = connection;
            _countdown = countdown;
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }
        // Function prints the numbers from 1 to the traget number that the user provided
        public void Listen()
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _countdown.Signal();
            };
            channel.BasicConsume(queue: "hello", autoAck: true, consumer: consumer);

        }
    }


    class Program
    {
        static CountdownEvent _countdown;
        
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.HostName = "localhost";
            var connection = factory.CreateConnection();

            Console.WriteLine("Enter number of messages to test with");
            int target = Convert.ToInt32(Console.ReadLine());
            int numOfPublishThreads = 1;
            int numOfConsumerThreads = 2;
            _countdown =  new CountdownEvent(target* numOfPublishThreads);
        
            Console.WriteLine("Starting");
            var stopwatch = Stopwatch.StartNew();
            // Start an array of consumers
            Thread[] consumerArray = new Thread[numOfConsumerThreads];
            for (int i = 0; i < consumerArray.Length; i++)
            {
                QueueConsumer consumer = new QueueConsumer(connection, _countdown);
                consumerArray[i] = new Thread(new ThreadStart(consumer.Listen));
                consumerArray[i].Start();
            }
            // Start an array of publishers
            Thread[] publisherArray = new Thread[numOfPublishThreads];
            for (int i = 0; i < publisherArray.Length; i++)
            {
                QueuePublisher publisher = new QueuePublisher( target, connection);
                publisherArray[i] = new Thread(new ThreadStart(publisher.Publish));
                publisherArray[i].Start();
            }

            for (int i = 0; i < publisherArray.Length; i++)
            {
                publisherArray[i].Join();
            }
            _countdown.Wait();
            Console.WriteLine($"DONE: {stopwatch.ElapsedMilliseconds}, {_countdown.CurrentCount}" );
        }
    }*/

    class Program
    {
        static CountdownEvent _countdown;
        static ServiceProvider provider;
        static void Main(string[] args)
        {
            // .Net Core DI Container
            var services = new ServiceCollection();
            // RMQ Specific
            var factory = new ConnectionFactory();
            factory.HostName = "localhost";
            var connection = factory.CreateConnection();
            // Store Connection in DI
            services.AddSingleton<IConnection>(connection);
            // Connection Params in DI
            services.AddSingleton<ActionArgs>(_ => new ActionArgs() { Queue = "HelloWorld" });
            // Add RMQ Publisher into DI - For Azure Service Bus it will be a different impl
            services.AddTransient<IPublisher,Publisher>();
            // Add RMQ Consumer into DI - For Azure Service Bus it will be a different impl
            services.AddTransient<IConsumer,Consumer>();
            provider = services.BuildServiceProvider();
            // The below does NOT change for RMQ or ASB
      /*      IPublisher publisher = provider.GetRequiredService<IPublisher>();
            IConsumer consumer = provider.GetRequiredService<IConsumer>();
            string message = "sdjhflsdkjhflsadkfhsldjfhsdlkjfhsdlkjfhsdlkjfhsdlkjfhsdlkjfhsdlkfjhdlkjgfhdljghdlghsdlfkjghdlkgjhdflkjghdlkjghdljghdlfkjghdlskjghdlkjghdlfkjghdlkjghdlkfjghdslfkjghdslkfjghdlskjghdlkgjhdflkjghdlfkjghdlkfjghdflkgjhdsflkjghdlfkgjhdlfjghdslkgjhdslgjhdslgjhsdlgjhdslkgjhdslkgjhdlkgjhdlfjghdlfkjghdlkfjghdlfgjhdlfkjghldfjghlsdfjghdlfgjhdslkfgjhdlkfjghdslkfhgdlkfsjghdslkfjghldksfjghdlfkgjhdlskfgjhdlkgjhdslkjghdslkjghldskjghlsdkgjhsdlkghsdlkghdslkfghjdlfjghdlfjghldfjghldkhglksdjhgfsdljhgsldkgjhdslkgjhdslfkjghdslkfgjhsdlkgjhsdlkfgjhsdlfkjghdslfkjghsdlkgjhdslkfjghdslkfjghdslkfjghdsklfjghdslfkjghdflkjghsldkgjhdslkfgjhsdlkgjhdlkfgjhdlgkjhdlkfgjhdflkgjhdflkgjhdlkgjhdglkjhglkdfsjhgdlfkjghldskjghdslkjgfhsdklfjghdlkfjghdlfkjghslkgjhdlfkjghsdlkfjgh";
            var body = Encoding.UTF8.GetBytes(message);
            consumer.ReceiveAsync((x) =>
            {
                var message = Encoding.UTF8.GetString(x);
                Console.WriteLine(message);
                return true;
            });
            publisher.Publish(body);*/



            int numOfPublishThreads = 1;
            int numOfConsumerThreads = 2;
            _countdown = new CountdownEvent(1000000 * numOfPublishThreads);

            Console.WriteLine("Starting");
            var stopwatch = Stopwatch.StartNew();
            // Start an array of consumers
            Thread[] consumerArray = new Thread[numOfConsumerThreads];
            for (int i = 0; i < consumerArray.Length; i++)
            {
                consumerArray[i] = new Thread(new ThreadStart(StartConsumer));
                consumerArray[i].Start();
            }
            // Start an array of publishers
            Thread[] publisherArray = new Thread[numOfPublishThreads];
            for (int i = 0; i < publisherArray.Length; i++)
            {
                publisherArray[i] = new Thread(new ThreadStart(StartPublisher));
                publisherArray[i].Start();
            }

            for (int i = 0; i < publisherArray.Length; i++)
            {
                publisherArray[i].Join();
            }
            _countdown.Wait();
            Console.WriteLine($"DONE: {stopwatch.ElapsedMilliseconds}, {_countdown.CurrentCount}");
        }

        static void StartConsumer ()
        {
            IConsumer consumer = provider.GetRequiredService<IConsumer>();
            consumer.ReceiveAsync((x) =>
            {
                var message = Encoding.UTF8.GetString(x);
                //Console.WriteLine(message);
                _countdown.Signal();
                return true;
            });
        }

        static void StartPublisher()
        {
            IPublisher publisher = provider.GetRequiredService<IPublisher>();
            var tickStart = Environment.TickCount64;
            string message = "sdjhflsdkjhflsadkfhsldjfhsdlkjfhsdlkjfhsdlkjfhsdlkjfhsdlkjfhsdlkfjhdlkjgfhdljghdlghsdlfkjghdlkgjhdflkjghdlkjghdljghdlfkjghdlskjghdlkjghdlfkjghdlkjghdlkfjghdslfkjghdslkfjghdlskjghdlkgjhdflkjghdlfkjghdlkfjghdflkgjhdsflkjghdlfkgjhdlfjghdslkgjhdslgjhdslgjhsdlgjhdslkgjhdslkgjhdlkgjhdlfjghdlfkjghdlkfjghdlfgjhdlfkjghldfjghlsdfjghdlfgjhdslkfgjhdlkfjghdslkfhgdlkfsjghdslkfjghldksfjghdlfkgjhdlskfgjhdlkgjhdslkjghdslkjghldskjghlsdkgjhsdlkghsdlkghdslkfghjdlfjghdlfjghldfjghldkhglksdjhgfsdljhgsldkgjhdslkgjhdslfkjghdslkfgjhsdlkgjhsdlkfgjhsdlfkjghdslfkjghsdlkgjhdslkfjghdslkfjghdslkfjghdsklfjghdslfkjghdflkjghsldkgjhdslkfgjhsdlkgjhdlkfgjhdlgkjhdlkfgjhdflkgjhdflkgjhdlkgjhdglkjhglkdfsjhgdlfkjghldskjghdslkjgfhsdklfjghdlkfjghdlfkjghslkgjhdlfkjghsdlkfjgh";
            var body = Encoding.UTF8.GetBytes(message);
            for (int x = 0; x <= 1000000; x++)
            {
                publisher.Publish(body);
            }
            var tickEnd = Environment.TickCount64;
            Console.WriteLine($"Published {1000000} messages in {tickEnd - tickStart} ms");
        }
    }
}
