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
        public RMQServer(string pHostname, string pUsername, string pPassword) : base(pHostname, pUsername, pPassword)
        {
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
