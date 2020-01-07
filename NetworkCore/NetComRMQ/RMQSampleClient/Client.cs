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

            client.BasicConsume();

            client.BasicExchanges();

            while (true)
            {
                Thread.Sleep(333);
                client.Send("", $"Hallo von C nach S at {DateTime.Now.ToLongTimeString()}", "LHFullBroadcast");
                Thread.Sleep(333);
                client.Send("", $"Hallo von C nach S at {DateTime.Now.ToLongDateString()}", "LHFullBroadcast");
                Thread.Sleep(333);
                client.Send("", $"Hallo von C nach S at {DateTime.Now.ToUniversalTime()}", "LHFullBroadcast");
            }
                
        }

        public static void OnReceive(object sender, BasicDeliverEventArgs e)
        {
            Console.WriteLine(Encoding.UTF8.GetString(e.Body));
        }
    }
}
