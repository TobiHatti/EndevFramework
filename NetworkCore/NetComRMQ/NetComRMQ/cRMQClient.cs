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

        /// <summary>
        /// Creates a new Rabbit-MQ client.
        /// A single user can log into multiple 
        /// client-instances without any problems.
        /// </summary>
        /// <param name="pHostname">Hostname of the RabbitMQ-Broker</param>
        /// <param name="pUsername">RMQ-Username</param>
        /// <param name="pPassword">RMQ-Password</param>
        public RMQClient(string pHostname, string pUsername, string pPassword) : base(pHostname, pUsername, pPassword)
        { 
            // Declare name of local queue
            // One client can create multiple queues, so the queue auto-deletes.
            // Client-queues have an additional ID
            localQueue = $"Q.Client.{pUsername}.{Guid.NewGuid()}";

            // Declare the Client-Queue.
            DeclareQueue(localQueue, true, false, true);
        }

        /// <summary>
        /// Binds the clients own queue to default and additional exchanges. 
        /// By default, the client subscribes to the Broadcast and ClientBroadcast
        /// exchanges
        /// </summary>
        /// <param name="pAdditionalExchanges">Names of additional exchanges</param>
        public override void BasicExchanges(params string[] pAdditionalExchanges)
        {
            base.BasicExchanges(pAdditionalExchanges);

            QueueBind(localQueue, excClientBroadcast);
        }
    }
}
