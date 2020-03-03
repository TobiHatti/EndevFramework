using NetComRMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXA_RMQ_Client
{
    class Client
    {
        static void Main(string[] args)
        {
            bool createRMQInfrastructure = true;

            RMQClient client = new RMQClient("localhost", "RMQClient", "adgjl");

            if (createRMQInfrastructure)
            {
                client.DeclareExchange("Broadcast", "fanout", true, false);
                client.DeclareExchange("BCPublishers", "fanout", true, false);
                client.DeclareExchange("BCSubscribers", "fanout", true, false);                
            }

            client.ExchangeSubscribeSelf("Broadcast");
            client.ExchangeSubscribeSelf("BCSubscribers");

            client.ReceiveMessageEvent(OnMessageReceive);
            client.ReceiveRequestEvent(OnRequestReceive);

            /* END OF SETUP */
            /* START OF SENDING */

            Console.WriteLine("Sending Message to Server...");
            client.SendTo("Hello Server!", "", client.ServerQueue);

            Console.WriteLine("Requesting Time from Server...");
            string time = client.RequestFrom("GetTime", client.ServerQueue);
            Console.WriteLine("Reply from Server: " + Convert.ToString(time));

            /* END OF SENDING */

            Console.ReadLine();
            client.Close();
        }

        private static string OnRequestReceive(string pIncommingMessage)
        {
            return pIncommingMessage;
        }

        private static void OnMessageReceive(string pIncommingMessage)
        {
            Console.WriteLine("Received Message: " + pIncommingMessage);
        }
    }
}
