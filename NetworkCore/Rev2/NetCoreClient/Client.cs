using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreClient
{
    class Client
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.HostName = "localhost";

            using (var con = factory.CreateConnection())
            {
                using(var channel = con.CreateModel())
                {
                    channel.QueueDeclare(
                        queue: "hallo",
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                        );

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += Consumer_Received;

                    channel.BasicConsume(
                        queue: "hallo",
                        autoAck: true,
                        consumer: consumer
                        );

                    Console.ReadLine();
                }
            }
        }

        private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var body = e.Body;
            var message = Encoding.UTF8.GetString(body);

            Console.WriteLine("Received message: {0}", message);
        }
    }
}
