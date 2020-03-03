using NetComRMQ;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOC_RMQ_Server
{
    class Server
    {
        static void Main(string[] args)
        {
            // Setting up the Rabbit-MQ Server (Publisher-Client)
            // • The server receives messages, even if it is offline.
            //   the received messages get processed once the server
            //   is started again.

            // Initializing the Server (Publisher)
            // On initialization, the server automatically creates a queue AND consumes it.
            RMQServer server = new RMQServer("localhost", "sampleUser", "samplePassword");

            // ====== RMQ-Receive Events ======

            // This handler can send 2 general types of messages:
            // 1) Simple-Messages: Gets sent from A To B, B does not reply and A 
            //    does not wait until a reply is received
            // 2) Request-Message: A sends a request to B, B replies to A. 
            //    A is halted until the reply is received.
            // Both of the following events need to be set to ensure propper use.

            // To set the receive-event for simple-messages, use
            server.ReceiveMessageEvent(MyMessageReceiveEvent);

            // To set the receive-event for request-messages, use
            server.ReceiveRequestEvent(MyRequestReceiveEvent);

            // ====== Initialising RMQ-Infrastructure ======

            // Define an exchange to distribute messages.
            // Option A) Declare the exchanges once in the RMQ-Management Window (Web-Interface) 
            // Option B) Declare the exchange programatically. Doesn't matter if the exchange already exists.
            //           The following line creates an exchange named "Broadcast" which distributes
            //           messages to all connected clients (if they are subscribed to this exchange).
            // The server does not receive messages from this exchange yet.
            server.DeclareExchange("Broadcast", "fanout", true, false);

            // The server already has a queue which gets consumed. if however another queue
            // is required, it can be done as shown below.
            // The following example creates a new queue named "SecondServerQueue".
            // The server does not consume the queue yet.
            server.DeclareQueue("SecondServerQueue", false, false, true);

            // ====== Binding RMQ-Infrastructure ======

            // To get messages from another queue besides the servers own queue, 
            // it needs to be consumed by the server:
            server.ConsumeQueue("SecondServerQueue");

            // To receive messages from an exchange (subscribe), it needs to be bound to a queue.
            // To bind the servers own queue to an exchange named "Broadcast", use:
            server.ExchangeSubscribeSelf("Broadcast");

            // To bind another queue to an exchange named "Broadcast", use
            server.ExchangeSubscribe("Broadcast", "SecondServerQueue");

            // *** End of Startup- and Setup-Section ***


            // ====== RMQ-Send Events ======
           
            // Send a simple message without a reply to a remote node:
            server.SendTo("MyMessageToSend", "Broadcast");

            // Send a request to a remote node. Waits until a reply is received OR the request times out.
            // Default Timeout-Duration is 5 seconds.
            string myReply = server.RequestFrom("MyThingIWantToRequest", "UserIWantToRequestFrom");

            // In case of slow internet-connections, large transfer files or intensive processing 
            // at the remote node, the timeout-duration can be increased. Time in Miliseconds
            RMQServer.TimeoutDuration = 10000;


            // ====== Closing an RMQ-Client ======

            // Closing the RMQ-Client is required to properly shut down any existing 
            // connections and to avoid errors upon the next start of the program.
            server.Close();


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
