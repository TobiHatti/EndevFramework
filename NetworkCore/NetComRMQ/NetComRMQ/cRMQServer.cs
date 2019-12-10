using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetComRMQ
{
    public class RMQServer
    {
        private ConnectionFactory factory = null;
        private IConnection connection = null;
        private IModel channel = null;
        private IBasicProperties basicProperties = null;
        public EventingBasicConsumer consumer = null;

        public EventingBasicConsumer Consumer
        {
            get => consumer;
            set => consumer = value;
        }

        public RMQServer(string pHostname)
        {
            factory = new ConnectionFactory();
            factory.HostName = pHostname;
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            basicProperties = channel.CreateBasicProperties();
        }

        public void DeclareQueue(string pQueueName, bool pIsDurable = false, bool pIsExclusive = false, bool pAutoDelete = false, IDictionary<string, object> pArguments = null)
        {
            channel.QueueDeclare(
                queue: pQueueName,
                durable: pIsDurable,
                exclusive: pIsExclusive,
                autoDelete: pAutoDelete,
                arguments: pArguments
            );
        }

        public void ConsumeQueue(string pQueueName, bool pAutoAck = false)
        {
            channel.BasicConsume(
                queue: pQueueName,
                autoAck: pAutoAck,
                consumer: consumer
            );
        }

        public void ReceiveEvent(event )

        public bool Send(string pRoutingKey, string pMessage, string pExchange = "")
        {
            try
            {
                channel.BasicPublish(
                    exchange: pExchange,
                    routingKey: pRoutingKey,
                    basicProperties: basicProperties,
                    body: Encoding.UTF8.GetBytes(pMessage)
                );

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
