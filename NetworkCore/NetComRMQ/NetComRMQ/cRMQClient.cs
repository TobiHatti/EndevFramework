using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetComRMQ
{
    public class RMQClient : RMQOperator
    {


        public RMQClient(string pHostname, string pUsername, string pPassword) : base(pHostname)
        {
            // Set the username and password
            factory.UserName = pUsername;
            factory.Password = pPassword;


            // Try to connect to the broker
            try
            {
                connection = factory.CreateConnection();
            }
            catch
            {
                throw new ApplicationException("*** Could not initialize. Check if the Username and/or Password are correct and if the server is set correctly. ***");
            }

            // Initialize the communication channel
            channel = connection.CreateModel();
            basicProperties = channel.CreateBasicProperties();

            // Declare name of local queue
            // One client can create multiple queues, so the queue auto-deletes.
            // Client-queues have an additional ID
            localQueue = $"Q.Client.{pUsername}.{Guid.NewGuid()}";

            // Declare the Client-Queue.
            DeclareQueue(localQueue, true, false, true);
        }

        public override void BasicExchanges(params string[] pAdditionalExchanges)
        {
            base.BasicExchanges(pAdditionalExchanges);

            QueueBind(localQueue, excClientBroadcast);
        }
    }
}
