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
            var stopwatch = Stopwatch.StartNew();
            Console.WriteLine("Starting");
            InitializeConsumersAndProducers(1, 2, 1000);
            _countdown.Wait();
            Console.WriteLine($"DONE: {stopwatch.ElapsedMilliseconds}, {_countdown.CurrentCount}");
        }
        /// <summary>
        /// This is messaging implementation agnostic
        /// </summary>
        /// <param name="numOfPublishThreads"></param>
        /// <param name="numOfConsumerThreads"></param>
        static void InitializeConsumersAndProducers (int numOfPublishThreads = 1, int numOfConsumerThreads = 2, int numOfMessages=1000)
        {
            _countdown = new CountdownEvent(numOfMessages * numOfPublishThreads);
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
                publisherArray[i] = new Thread(StartPublisher);
                publisherArray[i].Start(numOfMessages);
            }

            for (int i = 0; i < publisherArray.Length; i++)
            {
                publisherArray[i].Join();
            }
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

        static void StartPublisher(object data)
        {
            IPublisher publisher = provider.GetRequiredService<IPublisher>();
            var tickStart = Environment.TickCount64;
            string message = "sdjhflsdkjhflsadkfhsldjfhsdlkjfhsdlkjfhsdlkjfhsdlkjfhsdlkjfhsdlkfjhdlkjgfhdljghdlghsdlfkjghdlkgjhdflkjghdlkjghdljghdlfkjghdlskjghdlkjghdlfkjghdlkjghdlkfjghdslfkjghdslkfjghdlskjghdlkgjhdflkjghdlfkjghdlkfjghdflkgjhdsflkjghdlfkgjhdlfjghdslkgjhdslgjhdslgjhsdlgjhdslkgjhdslkgjhdlkgjhdlfjghdlfkjghdlkfjghdlfgjhdlfkjghldfjghlsdfjghdlfgjhdslkfgjhdlkfjghdslkfhgdlkfsjghdslkfjghldksfjghdlfkgjhdlskfgjhdlkgjhdslkjghdslkjghldskjghlsdkgjhsdlkghsdlkghdslkfghjdlfjghdlfjghldfjghldkhglksdjhgfsdljhgsldkgjhdslkgjhdslfkjghdslkfgjhsdlkgjhsdlkfgjhsdlfkjghdslfkjghsdlkgjhdslkfjghdslkfjghdslkfjghdsklfjghdslfkjghdflkjghsldkgjhdslkfgjhsdlkgjhdlkfgjhdlgkjhdlkfgjhdflkgjhdflkgjhdlkgjhdglkjhglkdfsjhgdlfkjghldskjghdslkjgfhsdklfjghdlkfjghdlfkjghslkgjhdlfkjghsdlkfjgh";
            var body = Encoding.UTF8.GetBytes(message);
            for (int x = 0; x <= (int)data; x++)
            {
                publisher.Publish(body);
            }
            var tickEnd = Environment.TickCount64;
            Console.WriteLine($"Published {(int)data} messages in {tickEnd - tickStart} ms");
        }
    }
}
