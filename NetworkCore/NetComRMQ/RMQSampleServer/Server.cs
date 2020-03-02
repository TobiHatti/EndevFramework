using NetComRMQ;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace RMQSampleServer
{
    class Server
    {
        static void Main(string[] args)
        {
            RMQServer server = new RMQServer("localhost", "RMQServer", "adgjl");

            server.ReceiveEvent(OnReceive);

            server.SelfConsume();

            server.BasicExchanges();

            server.ExchangeSubscribe("LHFullBroadcast", "Q.Server");

            while(true)
            {
                Thread.Sleep(500);
                server.SendTo("*", $"Hallo von S nach C at {DateTime.Now.ToLongTimeString()}", "LHFullBroadcast");
                Thread.Sleep(500);
                server.SendTo("*", $"Hallo von S nach C at {DateTime.Now.ToLongDateString()}", "LHFullBroadcast");
            }
                
        }
    

        public static void OnReceive(object sender, BasicDeliverEventArgs e)
        {
            Console.WriteLine(Encoding.UTF8.GetString(e.Body));
        }
    }


}
