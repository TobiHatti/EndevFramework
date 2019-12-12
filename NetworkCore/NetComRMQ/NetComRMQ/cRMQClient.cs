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
        public RMQClient(string pHostname) : base(pHostname)
        {
            factory.UserName = "RMQClient";
            factory.Password = "adgjl";

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            basicProperties = channel.CreateBasicProperties();
        }
    }
}
