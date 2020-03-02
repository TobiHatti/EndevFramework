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
            RMQClient client = new RMQClient("localhost", "RMQClient", "adgjl");

            client.ReceiveEvent(OnReceive);
            client.SelfConsume();
            client.BasicExchanges();

            while (true)
            {
                Thread.Sleep(500);
                client.SendTo($"Hallo von C nach S at {DateTime.Now.ToLongTimeString()}", "LHFullBroadcast", "*");
                Thread.Sleep(500);
                client.SendTo($"Hallo von C nach S at {DateTime.Now.ToLongTimeString()}", "LHFullBroadcast", "*");
            }
                
        }

        public static void OnReceive(object sender, BasicDeliverEventArgs e)
        {
            Console.WriteLine(Encoding.UTF8.GetString(e.Body));
        }
    }
}
