using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetComRMQ
{
    public abstract class RMQOperator
    {
        protected ConnectionFactory factory = null;
        protected IConnection connection = null;
        protected IModel channel = null;
        protected IBasicProperties basicProperties = null;
        protected EventingBasicConsumer consumer = null;

        protected string localQueue = null;

        protected const string excFullBroadcast = "LHFullBroadcast";
        protected const string excServerBroadcast = "LHServerBroadcast";
        protected const string excClientBroadcast = "LHClientBroadcast";

        /// <summary>
        /// Creates a new RabbitMQ-Operator. Connects to the broker, logs the user in and initializes the com-channel.
        /// </summary>
        /// <param name="pHostname">Hostname of the RabbitMQ-Broker</param>
        /// <param name="pUsername">RMQ-Username</param>
        /// <param name="pPassword">RMQ-Password</param>
        public RMQOperator(string pHostname, string pUsername, string pPassword)
        {
            factory = new ConnectionFactory();
            factory.HostName = pHostname;

            // Set the username and password
            factory.UserName = pUsername;
            factory.Password = pPassword;

            // Try to connect to the broker
            try { connection = factory.CreateConnection(); }
            catch { throw new ApplicationException("*** Could not initialize. Check if the Username and/or Password are correct and if the server is set correctly. ***"); }

            // Initialize the communication channel
            channel = connection.CreateModel();
            basicProperties = channel.CreateBasicProperties();
        }

        /// <summary>
        /// Propertly closes all connections to the RabbitMQ-Broker.
        /// Required before exiting the program.
        /// </summary>
        public void Close()
        {
            channel.Close();
            connection.Close();
        }

        /// <summary>
        /// Declares a new Exchange
        /// </summary>
        /// <param name="pExchangeName">Name of the exchange</param>
        /// <param name="pExchangeType">Type of the exchange. Valid types: "direct","topic","fanout","headers"</param>
        /// <param name="pDurable">If true, the exchange will survive a broker restart</param>
        /// <param name="pAutoDelete">If true, the exchange is deleted when the last consumer unsubscribes</param>
        /// <param name="pArguments">Optional; used by plugins and broker-specific features</param>
        public void DeclareExchange(string pExchangeName, string pExchangeType, bool pDurable = false, bool pAutoDelete = true, Dictionary<string, object> pArguments = null)
        {
            channel.ExchangeDeclare(
                exchange: pExchangeName,
                type: pExchangeType
            );
        }

        /// <summary>
        /// Declares a new Queue
        /// </summary>
        /// <param name="pQueueName">Name of the queue</param>
        /// <param name="pIsDurable">If true, the queue will survive a broker restart</param>
        /// <param name="pIsExclusive">Used by only one connection and the queue will be deleted when that connection closes</param>
        /// <param name="pAutoDelete">If true, the queue that has had at least one consumer is deleted when last consumer unsubscribes</param>
        /// <param name="pArguments">Optional; used by plugins and broker-specific features</param>
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

        /// <summary>
        /// Declares the consumtion of a queue
        /// </summary>
        /// <param name="pQueueName">Name of the queue that should be consumed</param>
        /// <param name="pAutoAck">Send an automated acknowledged-information to the broker once the message is received</param>
        public void ConsumeQueue(string pQueueName, bool pAutoAck = true)
        {
            channel.BasicConsume(
                queue: pQueueName,
                autoAck: pAutoAck,
                consumer: consumer
            );
        }

        /// <summary>
        /// Subscribes a queue to an exchange
        /// </summary>
        /// <param name="pQueueName">Name of the queue</param>
        /// <param name="pExchangeName">Name of the exchange</param>
        /// <param name="pRoutingKey">Optional. Routing-Key for additional and complex message routing</param>
        public void QueueBind(string pQueueName, string pExchangeName, string pRoutingKey = "")
        {
            channel.QueueBind(queue: pQueueName,
                  exchange: pExchangeName,
                  routingKey: pRoutingKey
            );
        }

        /// <summary>
        /// Sets the event when a message is received.
        /// </summary>
        /// <param name="pReceiveEvent">Receive-Event</param>
        public void ReceiveEvent(EventHandler<BasicDeliverEventArgs> pReceiveEvent)
        {
            consumer = new EventingBasicConsumer(channel);
            consumer.Received += pReceiveEvent;
        }

        /// <summary>
        /// Consumes the Operators own queue (default use scenario)
        /// </summary>
        public void BasicConsume()
        {
            ConsumeQueue(localQueue);
        }

        /// <summary>
        /// Binds the operators own queue to the given exchanges
        /// </summary>
        /// <param name="pAdditionalExchanges">Names of the exchanges</param>
        public virtual void BasicExchanges(params string[] pAdditionalExchanges)
        {
            QueueBind(localQueue, excFullBroadcast);

            foreach(string exc in pAdditionalExchanges)
                QueueBind(localQueue, exc);
        }

        /// <summary>
        /// Sends a message to an exchange or directly to another queue
        /// </summary>
        /// <param name="pRoutingKey">Routing-Key for additional and complex message routing, or to send directly to another queue</param>
        /// <param name="pMessage">Message to be sent</param>
        /// <param name="pExchange">Target exchange to distribute the message</param>
        /// <returns>True in case the send-process was successfull</returns>
        public bool Send(string pMessage, string pExchange = "", string pRoutingKey = "")
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
