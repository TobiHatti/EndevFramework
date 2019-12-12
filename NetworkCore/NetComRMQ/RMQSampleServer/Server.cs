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
            RMQServer server = new RMQServer("localhost");
            
            server.DeclareQueue("Server2Client");
            server.DeclareQueue("Client2Server");

          

            server.ReceiveEvent(OnReceive);

            server.ConsumeQueue("Client2Server");

            server.DeclareExchange("BROADCAST", "fanout");

            server.QueueBind("Server2Client", "BROADCAST");

            while(true)
            {
                Thread.Sleep(500);
                server.Send("Server2Client", $"Hallo von S nach C at {DateTime.Now.ToLongTimeString()}", "BROADCAST");
                Thread.Sleep(500);
                server.Send("Server2Client", $"Hallo von S nach C at {DateTime.Now.ToLongDateString()}", "BROADCAST");
            }
                
        }
    

        public static void OnReceive(object sender, BasicDeliverEventArgs e)
        {
            Console.WriteLine(Encoding.UTF8.GetString(e.Body));
        }
    }


}
