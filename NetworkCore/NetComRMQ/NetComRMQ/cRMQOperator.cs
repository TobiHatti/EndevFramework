using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        protected const string messagePrefix = "[MSG:]";
        protected const string requestPrefix = "[RQT:]";
        protected const string replyPrefix = "[RPY:]";

        public delegate void MessageReceiveEvent(string pIncommingMessage);
        public delegate string RequestReceiveEvent(string pIncommingMessage);
        private MessageReceiveEvent receiveMessageEvent = null;
        private RequestReceiveEvent receiveRequestEvent = null;

        private List<RMQReplyElement> replies = new List<RMQReplyElement>();

        /// <summary>
        /// Sets the duration which has to expire until a request gets a timeout-reply.
        /// Time in miliseconds
        /// </summary>
        public static int TimeoutDuration { get; set; } = 5000;

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

            // Set the receive-event for incomming messages or requests
            consumer = new EventingBasicConsumer(channel);
            consumer.Received += ReceiveMessageOperationFilter;
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
        public void ExchangeSubscribe(string pExchangeName, string pQueueName, string pRoutingKey = "")
        {
            channel.QueueBind(queue: pQueueName,
                  exchange: pExchangeName,
                  routingKey: pRoutingKey
            );
        }

        /// <summary>
        /// Subscribes the nodes own queue to an exchange
        /// </summary>
        /// <param name="pExchangeName">Name of the exchange</param>
        /// <param name="pRoutingKey">Optional. Routing-Key for additional and complex message routing</param>
        public void ExchangeSubscribeSelf(string pExchangeName, string pRoutingKey = "")
        {
            channel.QueueBind(queue: localQueue,
                  exchange: pExchangeName,
                  routingKey: pRoutingKey
            );
        }

        /// <summary>
        /// Receives incomming messages, deconstructs them and assigns them to the propper receive-event.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event-Arguments</param>
        private void ReceiveMessageOperationFilter(object sender, BasicDeliverEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.Body);

            if (message.StartsWith(messagePrefix))
            {
                message = message.Substring(message.IndexOf(']') + 1);
                receiveMessageEvent(message);
            }
            else if (message.StartsWith(requestPrefix))
            {
                string senderQueue = "";
                string requestID = "";
                string requestReply = null;
                int replyStatusCode = 100;

                // Remove Request-Header
                message = message.Substring(requestPrefix.Length);

                // Check if a Sender-Queue-Tag exists
                if(message.StartsWith("[SQ:"))
                {
                    // Get Sender-Queue
                    senderQueue = message.Substring(4, message.IndexOf(']') - 4);

                    // Remove Sender-Queue from Message
                    message = message.Substring(message.IndexOf(']') + 1);
                }

                if(message.StartsWith("[MID:"))
                {
                    // Get Request-ID
                    requestID = message.Substring(5, message.IndexOf(']') - 5);

                    // Remove Request-ID from Message
                    message = message.Substring(message.IndexOf(']') + 1);
                }

                try
                {
                    requestReply = receiveRequestEvent(message);
                    if(string.IsNullOrEmpty(requestReply)) replyStatusCode = 200;
                }
                catch(Exception ex)
                {
                    replyStatusCode = 999;
                }
                finally
                {
                    SendReply($"[RSC:{Convert.ToString(replyStatusCode)}][MID:{requestID}]{Convert.ToString(requestReply)}", "", senderQueue);
                }
            }
            else if (message.StartsWith(replyPrefix))
            {
                string requestID = "";
                int statusCode = 0;              

                // Remove Request-Header
                message = message.Substring(replyPrefix.Length);

                // Check if a Sender-Queue-Tag exists
                if (message.StartsWith("[RSC:"))
                {
                    // Get Status-Code
                    statusCode = Convert.ToInt32(message.Substring(5, message.IndexOf(']') - 5));

                    // Remove Sender-Queue from Message
                    message = message.Substring(message.IndexOf(']') + 1);
                }

                if (message.StartsWith("[MID:"))
                {
                    // Get Request-ID
                    requestID = message.Substring(5, message.IndexOf(']') - 5);

                    // Remove Request-ID from Message
                    message = message.Substring(message.IndexOf(']') + 1);
                }

                replies.Add(new RMQReplyElement(message, statusCode, Guid.Parse(requestID)));
            }
            else throw new ApplicationException("Error: No Messagetype-Prefix detected. Please ensure that both connected clients use the same version of this library");
        }

        /// <summary>
        /// Sets the event which gets called when a message is received
        /// </summary>
        /// <param name="pReceiveEvent">Delegate to the receive-message method</param>
        public void ReceiveMessageEvent(MessageReceiveEvent pReceiveEvent)
        {
            receiveMessageEvent = pReceiveEvent;
        }

        /// <summary>
        /// Sets the event which gets called when a request is received
        /// </summary>
        /// <param name="pReceiveEvent">Delegate to the receive-event method</param>
        public void ReceiveRequestEvent(RequestReceiveEvent pReceiveEvent)
        {
            receiveRequestEvent = pReceiveEvent;
        }

        /// <summary>
        /// Sends a message to an exchange or directly to another queue
        /// </summary>
        /// <param name="pMessage">Message to be sent</param>
        /// <param name="pRoutingKey">Routing-Key for additional and complex message routing, or to send directly to another queue</param>
        /// <param name="pExchange">Target exchange to distribute the message</param>
        /// <returns>True in case the send-process was successfull</returns>
        public bool SendTo(string pMessage, string pExchange = "", string pRoutingKey = "")
        {
            // Check if either an exchange or a routing key is provided
            if (string.IsNullOrEmpty(pExchange) && string.IsNullOrEmpty(pRoutingKey))
                throw new ApplicationException("Error in SendTo(): Please provide either a Exchange OR a RoutingKey, leaving both parameters blank is not possible");

            // Re-Format message to fit protocol
            pMessage = $"{messagePrefix}{pMessage}";

            // Try to send the message
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
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                return false;
            }
        }

        private bool SendReply(string pMessage, string pExchange = "", string pRoutingKey = "")
        {
            // Check if either an exchange or a routing key is provided
            if (string.IsNullOrEmpty(pExchange) && string.IsNullOrEmpty(pRoutingKey))
                throw new ApplicationException("Error in SendTo(): Please provide either a Exchange OR a RoutingKey, leaving both parameters blank is not possible");

            // Re-Format message to fit protocol
            pMessage = $"{replyPrefix}{pMessage}";

            // Try to send the message
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
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Requests a value or object from a remote node.
        /// </summary>
        /// <param name="pRequestInstruction">Request-String</param>
        /// <param name="pRoutingKey">Routing-Key for additional and complex message routing, or to send directly to another queue</param>
        /// <param name="pExchange">Target exchange to distribute the message</param>
        /// <returns>Returns the result of the request</returns>
        public string RequestFrom(string pRequestInstruction, string pRoutingKey)
        {
            // Generate unique ID for request
            Guid requestID = Guid.NewGuid();

            // Re-Format message to fit protocol
            pRequestInstruction = $"{requestPrefix}[SQ:{this.localQueue}][MID:{requestID}]{pRequestInstruction}";

            // Try to send the message
            try
            {
                channel.BasicPublish(
                    exchange: "",
                    routingKey: pRoutingKey,
                    basicProperties: basicProperties,
                    body: Encoding.UTF8.GetBytes(pRequestInstruction)
                );
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

            int cyclePauseTimeMS = 100;
            int maxCyclesUntilTimeout = (int)Math.Ceiling((decimal)TimeoutDuration / cyclePauseTimeMS);
            int cycleCounter = 0;

            string replyValue = null;
            int statusCode = 0;
            bool replyFound = false;

            // Cycle through the received replies until a request is received
            while (!replyFound && cycleCounter++ < maxCyclesUntilTimeout)
            {
                if(replies.Count > 0)
                {
                    for (int i = 0; i < replies.Count; i++)
                    {
                        if (!replyFound && replies[i].ID == requestID)
                        {
                            replyValue = replies[i].Message;
                            statusCode = replies[i].StatusCode;
                            replyFound = true;

                            replies.RemoveAt(i);
                        }
                    }
                }
                if (!replyFound) Thread.Sleep(cyclePauseTimeMS);
            }

            // Remove result from list
            if(replyFound) return replyValue;
            else return null;
        }

        internal class RMQReplyElement
        {
            internal string Message { get; set; } = null;
            internal Guid ID { get; set; } = new Guid();
            internal int StatusCode { get; set; } = 0;

            internal RMQReplyElement(string pMessage, int pStatusCode, Guid pID)
            {
                this.Message = pMessage;
                this.StatusCode = pStatusCode;
                this.ID = pID;
            }
        }

        #region Obsolete Methods

        protected const string excFullBroadcast = "LHFullBroadcast";
        protected const string excServerBroadcast = "LHServerBroadcast";
        protected const string excClientBroadcast = "LHClientBroadcast";

        /// <summary>
        /// Consumes the Operators own queue (default use scenario)
        /// </summary>
        [Obsolete("This Method is deprecated. Binding already occurs on initialisation.")]
        public void SelfConsume()
        {
            ConsumeQueue(localQueue);
        }

        /// <summary>
        /// Binds the operators own queue to the given exchanges. 
        /// </summary>
        /// <param name="pAdditionalExchanges">Names of additional exchanges</param>
        [Obsolete("This Method should not be used anymore. Too static use-case")]
        public virtual void BasicExchanges(params string[] pAdditionalExchanges)
        {
            DeclareExchange(excFullBroadcast, "fanout", true, false);
            ExchangeSubscribe(excFullBroadcast, localQueue);

            foreach (string exc in pAdditionalExchanges)
            {
                DeclareExchange(localQueue, "fanout", true, false);
                ExchangeSubscribe(exc, localQueue);
            }
        }

        /// <summary>
        /// Sets the event when a message is received. This method checks if the message is a 
        /// simple-message, request or reply and passes on the processing.
        /// </summary>
        /// <param name="pReceiveEvent">Receive-Event</param>
        [Obsolete("This Method is deprecated. Default Receive-Event is already set in constructor. Use ReceiveMessageEvent and ReceiveRequestEvent instead")]
        public void ReceiveEvent(EventHandler<BasicDeliverEventArgs> pReceiveEvent)
        {
            consumer = new EventingBasicConsumer(channel);
            consumer.Received += pReceiveEvent;
        }
        #endregion
    }
}
