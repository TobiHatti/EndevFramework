using NetComRMQ;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOC_RMQ_Client
{
    class Client
    {
        static void Main(string[] args)
        {
            // Setting up the Rabbit-MQ Client (Subscriber-Client)
            // • The client receives messages only if it is online.
            //   Messages that get sent to the client get lost when 
            //   the client is offline.

            // Initializing the Client (Consumer)
            // On initialization, the client automatically creates a queue AND consumes it.
            RMQClient client = new RMQClient("localhost", "sampleUser", "samplePassword");

            // ====== Initialising RMQ-Infrastructure ======

            // Define an exchange to distribute messages.
            // Option A) Declare the exchanges once in the RMQ-Management Window (Web-Interface) 
            // Option B) Declare the exchange programatically. Doesn't matter if the exchange already exists.
            //           The following line creates an exchange named "Broadcast" which distributes
            //           messages to all connected clients (if they are subscribed to this exchange).
            // The server does not receive messages from this exchange yet.
            client.DeclareExchange("Broadcast", "fanout", true, false);

            // The server already has a queue which gets consumed. if however another queue
            // is required, it can be done as shown below.
            // The following example creates a new queue named "SecondServerQueue".
            // The server does not consume the queue yet.
            client.DeclareQueue("SecondServerQueue", false, false, true);

            // ====== Binding RMQ-Infrastructure ======

            // To get messages from another queue besides the clients own queue, 
            // it needs to be consumed by the client:
            client.ConsumeQueue("SecondServerQueue");

            // To receive messages from an exchange (subscribe), it needs to be bound to a queue.
            // To bind the servers own queue to an exchange named "Broadcast", use:
            client.ExchangeSubscribeSelf("Broadcast");

            // To bind another queue to an exchange named "Broadcast", use
            client.ExchangeSubscribe("Broadcast", "SecondServerQueue");

            // ====== RMQ-Receive Events ======

            // This handler can send 2 general types of messages:
            // 1) Simple-Messages: Gets sent from A To B, B does not reply and A 
            //    does not wait until a reply is received
            // 2) Request-Message: A sends a request to B, B replies to A. 
            //    A is halted until the reply is received.
            // Both of the following events need to be set to ensure propper use.

            // To set the receive-event for simple-messages, use
            client.ReceiveMessageEvent(MyMessageReceiveEvent);

            // To set the receive-event for request-messages, use
            client.ReceiveRequestEvent(MyRequestReceiveEvent);

            // *** End of Startup- and Setup-Section ***


            // ====== RMQ-Send Events ======

            // Send a simple message without a reply to a remote node:
            client.SendTo("MyMessageToSend", "Broadcast");

            // Send a request to a remote node. Waits until a reply is received OR the request times out.
            // Default Timeout-Duration is 5 seconds.
            string myReply = client.RequestFrom("MyThingIWantToRequest", "UserIWantToRequestFrom");

            // In case of slow internet-connections, large transfer files or intensive processing 
            // at the remote node, the timeout-duration can be increased. Time in Miliseconds
            RMQServer.TimeoutDuration = 10000;


            // ====== Closing an RMQ-Client ======

            // Closing the RMQ-Client is required to properly shut down any existing 
            // connections and to avoid errors upon the next start of the program.
            client.Close();


            Console.WriteLine("Done");
        }

        private static string MyRequestReceiveEvent(string pIncommingMessage)
        {
            return pIncommingMessage.ToUpper();
        }

        private static void MyMessageReceiveEvent(string pIncommingMessage)
        {
            Console.WriteLine(pIncommingMessage);
        }
    }
}
