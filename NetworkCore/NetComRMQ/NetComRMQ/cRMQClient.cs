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
        /// 
        /// </summary>
        /// <param name="pHostname"></param>
        /// <param name="pUsername"></param>
        /// <param name="pPassword"></param>
        public RMQClient(string pHostname, string pUsername, string pPassword) : base(pHostname, pUsername, pPassword)
        { 
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
