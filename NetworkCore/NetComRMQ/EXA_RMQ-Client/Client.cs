using NetComRMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EXA_RMQ_Client
{
    class Client
    {
        static void Main(string[] args)
        {
            bool createRMQInfrastructure = true;

            RMQClient client = new RMQClient("localhost", "RMQClient", "adgjl");

            client.ReceiveMessageEvent(OnMessageReceive);
            client.ReceiveRequestEvent(OnRequestReceive);

            if (createRMQInfrastructure)
            {
                client.DeclareExchange("Broadcast", "fanout", true, false);
                client.DeclareExchange("BCPublishers", "fanout", true, false);
                client.DeclareExchange("BCSubscribers", "fanout", true, false);                
            }

            client.ExchangeSubscribeSelf("Broadcast");
            client.ExchangeSubscribeSelf("BCSubscribers");

            /* END OF SETUP */
            /* START OF SENDING */

            Thread.Sleep(5000);

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Sending Message to Server...");
                client.SendTo("Hello Server!", "", client.ServerQueue);

                Thread.Sleep(300);
            }

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Requesting Time from Server...");
                string time = client.RequestFrom("GetTime", client.ServerQueue);
                Console.WriteLine("Reply from Server: " + Convert.ToString(time));

                Thread.Sleep(300);
            }

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
