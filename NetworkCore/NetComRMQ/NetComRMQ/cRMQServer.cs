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
        /// <summary>
        /// Creates a new Rabbit-MQ server.
        /// A single user can only log into one 
        /// server-instances without any problems.
        /// </summary>
        /// <param name="pHostname">Hostname of the RabbitMQ-Broker</param>
        /// <param name="pUsername">RMQ-Username</param>
        /// <param name="pPassword">RMQ-Password</param>
        public RMQServer(string pHostname, string pUsername, string pPassword) : base(pHostname, pUsername, pPassword)
        {
            // Declare name of local queue
            localQueue = "Q.Server." + pUsername.ToLower();

            // Declare the Server-Queue
            DeclareQueue(localQueue, true, false, false);

            // Consume the local queue
            ConsumeQueue(localQueue);
        }

        /// <summary>
        /// Binds the servers own queue to default and additional exchanges. 
        /// By default, the server subscribes to the Broadcast and ServerBroadcast
        /// exchanges
        /// </summary>
        /// <param name="pAdditionalExchanges">Names of additional exchanges</param>
        [Obsolete("This Method should not be used anymore. Too static use-case")]
        public override void BasicExchanges(params string[] pAdditionalExchanges)
        {
            base.BasicExchanges(pAdditionalExchanges);

            ExchangeSubscribe(excServerBroadcast, localQueue);
        }
    }
}
