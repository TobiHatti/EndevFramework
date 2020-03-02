using NetComRMQ;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOC_RMQ_Client
{
    class Program
    {
        static void Main()
        {
            // Setting up the Rabbit-MQ Client (Consumer-Client)
            // • The client only receives messages if it is online. 
            //   Once the client is offline, the queue of the client
            //   gets deleted and no messages are received anymore.

            // Initialising the Client (Consumer)
            RMQClient client = new RMQClient("localhost", "sampleUser", "samplePassword");

            // Set the receive-event for incoming messages (One-Way-Messages)
            client.ReceiveEvent(MyCustomReceiveEvent);

        }

        // Sample Receive-Event for One-Way-Messages - Simply prints 
        // out the message to the console.
        public static void MyCustomReceiveEvent(object sender, BasicDeliverEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.Body);
            Console.WriteLine(message);
        }
    }
}
