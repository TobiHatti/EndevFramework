using NetComRMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXA_RMQ_Server
{
    class Server
    {
        static void Main(string[] args)
        {
            bool createRMQInfrastructure = true;

            RMQServer server = new RMQServer("localhost", "RMQServer", "adgjl");

            server.ReceiveMessageEvent(OnMessageReceive);
            server.ReceiveRequestEvent(OnRequestReceive);

            if (createRMQInfrastructure)
            {
                server.DeclareExchange("Broadcast", "fanout", true, false);
                server.DeclareExchange("BCPublishers", "fanout", true, false);
                server.DeclareExchange("BCSubscribers", "fanout", true, false);
            }

            server.ExchangeSubscribeSelf("Broadcast");
            server.ExchangeSubscribeSelf("BCPublishers");

            /* END OF SETUP */
            /* START OF SENDING */

            Console.WriteLine("Sending Message to All Clients...");
            server.SendTo("Hello Clients!", "BCSubscribers");

            /* END OF SENDING */

            Console.ReadLine();
            server.Close();
        }

        private static string OnRequestReceive(string pIncommingMessage)
        {
            switch(pIncommingMessage)
            {
                case "GetTime":
                    return DateTime.Now.ToLongTimeString();
                case "GetDate":
                    return DateTime.Now.ToLongDateString();
                default: 
                    return "Invalid Request!";
            }
        }

        private static void OnMessageReceive(string pIncommingMessage)
        {
            Console.WriteLine("Received Message: " + pIncommingMessage);
        }
    }
}
