using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreServer
{
    class Server
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.HostName = "localhost";

            using (var con = factory.CreateConnection())
            {
                using(var channel = con.CreateModel())
                {
                    var basicProperties = channel.CreateBasicProperties();
                    basicProperties.Persistent = true;

                    channel.QueueDeclare(
                        queue: "hallo",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                        );

                    var msg = "Hello World!";
                    var body = Encoding.UTF8.GetBytes(msg);

                   while(Console.ReadKey().Key == ConsoleKey.Enter)
                    {
                        channel.BasicPublish(
                            routingKey: "hallo",
                            exchange: "",
                            basicProperties: basicProperties,
                            body: body
                            );

                        Console.WriteLine("Message sent!");
                    }
                }
            }
        }
    }
}
