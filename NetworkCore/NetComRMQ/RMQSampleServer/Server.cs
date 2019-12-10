using NetComRMQ;
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
        }
    
        public static void OnReceive(object sender, BasicDeliverEventArgs e)
        {
            Console.WriteLine(Encoding.UTF8.GetString(e.Body));
        }
    }


}
