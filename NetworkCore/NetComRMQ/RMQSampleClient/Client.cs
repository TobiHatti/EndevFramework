using NetComRMQ;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace RMQSampleClient
{
    class Client
    {
        static void Main(string[] args)
        {
            RMQClient server = new RMQClient("localhost");

            server.DeclareQueue("Server2Client");
            server.DeclareQueue("Client2Server");

            server.ReceiveEvent(OnReceive);

            server.ConsumeQueue("Server2Client");

            while (true)
            {
                Thread.Sleep(333);
                server.Send("Client2Server", $"Hallo von C nach S at {DateTime.Now.ToLongTimeString()}");
                Thread.Sleep(333);
                server.Send("Client2Server", $"Hallo von C nach S at {DateTime.Now.ToLongDateString()}");
                Thread.Sleep(333);
                server.Send("Client2Server", $"Hallo von C nach S at {DateTime.Now.ToUniversalTime()}");
            }
                
        }

        public static void OnReceive(object sender, BasicDeliverEventArgs e)
        {
            Console.WriteLine(Encoding.UTF8.GetString(e.Body));
        }
    }
}
