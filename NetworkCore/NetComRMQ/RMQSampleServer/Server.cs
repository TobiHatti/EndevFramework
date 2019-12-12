using NetComRMQ;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            while(true)
                server.Send("Server2Client", "Hallo von S nach C");
        }
    
        public static void OnReceive(object sender, BasicDeliverEventArgs e)
        {
            Console.WriteLine(Encoding.UTF8.GetString(e.Body));
        }
    }


}
