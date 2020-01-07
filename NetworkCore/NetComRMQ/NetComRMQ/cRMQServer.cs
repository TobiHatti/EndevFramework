using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetComRMQ
{
    public class RMQServer : RMQOperator
    {
        public RMQServer(string pHostname, string pUsername, string pPassword) : base(pHostname)
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
            localQueue = "Q.Server";

            // Declare the Server-Queue
            DeclareQueue(localQueue, true, false, false);
        }

        public override void BasicExchanges(params string[] pAdditionalExchanges)
        {
            base.BasicExchanges(pAdditionalExchanges);

            QueueBind(localQueue, excServerBroadcast);
        }
    }
}
