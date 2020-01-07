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

            server.BasicConsume();

            server.BasicExchanges();

            server.QueueBind("Q.Server", "LHFullBroadcast");

            while(true)
            {
                Thread.Sleep(500);
                server.Send("*", $"Hallo von S nach C at {DateTime.Now.ToLongTimeString()}", "LHFullBroadcast");
                Thread.Sleep(500);
                server.Send("*", $"Hallo von S nach C at {DateTime.Now.ToLongDateString()}", "LHFullBroadcast");
            }
                
        }
    

        public static void OnReceive(object sender, BasicDeliverEventArgs e)
        {
            Console.WriteLine(Encoding.UTF8.GetString(e.Body));
        }
    }


}
