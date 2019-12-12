using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetComRMQ
{
    public class RMQOperator
    {
        protected ConnectionFactory factory = null;
        protected IConnection connection = null;
        protected IModel channel = null;
        protected IBasicProperties basicProperties = null;
        protected EventingBasicConsumer consumer = null;

        public RMQOperator(string pHostname)
        {
            factory = new ConnectionFactory();
            factory.HostName = pHostname;
        }

        public void DeclareExchange(string pExchangeName, string pExchangeType)
        {
            channel.ExchangeDeclare(
                exchange: pExchangeName,
                type: pExchangeType
            );
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

        public void ConsumeQueue(string pQueueName, bool pAutoAck = true)
        {
            channel.BasicConsume(
                queue: pQueueName,
                autoAck: pAutoAck,
                consumer: consumer
            );
        }

        public void QueueBind(string pQueueName, string pExchangeName, string pRoutingKey = "")
        {
            channel.QueueBind(queue: pQueueName,
                  exchange: pExchangeName,
                  routingKey: pRoutingKey
            );
        }

        public void ReceiveEvent(EventHandler<BasicDeliverEventArgs> pReceiveEvent)
        {
            consumer = new EventingBasicConsumer(channel);
            consumer.Received += pReceiveEvent;
        }

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
