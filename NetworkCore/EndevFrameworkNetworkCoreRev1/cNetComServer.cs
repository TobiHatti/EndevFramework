using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace EndevFrameworkNetworkCoreRev1
{
#pragma warning disable 0168

    public class NetComServer
    {
        //===================================================================================================================
        //===================================================================================================================
        //=         PROPERTIES / FIELDS / DELEGATES                                                                         =
        //===================================================================================================================
        //===================================================================================================================

        #region -=[- PROPERTIES -]=-
        public const int BufferSize = 1048576;
        public Socket ServerSocket { get; private set; } = null;
        public int Port { get; private set; }
        public NetComClientList<NetComClientData> LClients { get; private set; } = new NetComClientList<NetComClientData>();
        private byte[] Buffer { get; set; } = new byte[BufferSize];    // 1 MB
        private byte[] Data { get; set; } = null;
        public NetComInstructionQueue<string, Socket> IncommingInstructions { get; private set; } = new NetComInstructionQueue<string, Socket>();
        public NetComInstructionQueue<string, Socket> OutgoingInstructions { get; set; } = new NetComInstructionQueue<string, Socket>();
        public bool SendStreamOpen { get; private set; } = true;
        public int ThreadSleep { get; set; } = 100;

        private Thread CommandProcessingThread = null;
        private Thread CommandSendingThread = null;

        #endregion

        #region -=[- DELEGATES -]=-

        // Debug output
        public delegate void DebugOutput(string pMessage, params object[] pParameters);
        public DebugOutput Debug { get; set; } = null;
        public object[] DebugParams { get; set; } = null;
        // Message Parser
        public delegate string MessageParser(string pMessage, params object[] pParameters);
        public MessageParser ParseMessage { get; set; } = null;

        // Message Encoder
        public delegate string MessageEncoder(string pMessage, params object[] pParameters);
        public MessageEncoder EncodeMessage { get; set; } = null;

        // Message Library
        public delegate object[] MessageLibraryExec(string pMessageKey, params object[] pParameters);
        public MessageLibraryExec LibraryExec { get; set; } = null;

        #endregion

        //===================================================================================================================
        //===================================================================================================================
        //=         CONSTRUCTOR                                                                                             =
        //===================================================================================================================
        //===================================================================================================================

        #region -=[- CONSTRUCTOR -]=-

        public NetComServer(int pPort)
        {
            Port = pPort;

            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        #endregion

        //===================================================================================================================
        //===================================================================================================================
        //=         CALLABLE METHODS                                                                                        =
        //===================================================================================================================
        //===================================================================================================================

        #region -=[- CALLABLE METHODS -]=-

        //-------------------------------------------------------------------------------------------------------------------
        //-         GENERAL / STARTUP                                                                                       -
        //-------------------------------------------------------------------------------------------------------------------

        #region -=[- GENERAL / STARTUP -]=-

        /// <summary>
        /// Initializes the TCP-Port for communication
        /// </summary>
        public void Init()
        {
            if (Debug == null || ParseMessage == null || EncodeMessage == null || LibraryExec == null)
                throw new NetComException("Error: Every delegate must be set! (Debug, ParseMessage, EncodeMessage, LibraryExec)");

            Debug("Initializing...", DebugParams);
            ServerSocket.Bind(new IPEndPoint(IPAddress.Any, Port));
            ServerSocket.Listen(20);
            Debug("Initializing Done!", DebugParams);
        }

        /// <summary>
        /// Opens the port and start accepting connections
        /// </summary>
        public void Start()
        {
            Debug("Opening Socket...", DebugParams);
            ServerSocket.BeginAccept(new AsyncCallback(OnClientAccept), null);
        }

        #endregion

        //-------------------------------------------------------------------------------------------------------------------
        //-         PROCESSING                                                                                              -
        //-------------------------------------------------------------------------------------------------------------------

        #region -=[- PROCESSING -]=-

        public void EnableProcessing()
        {
            CommandProcessingThread = new Thread(ProcessInstructionLoop);
            CommandProcessingThread.Start();
        }

        private void ProcessInstructionLoop()
        {
            while (true)
            {
                if (IncommingInstructions.Count > 0) ProcessNextQueueItem();
                Thread.Sleep(ThreadSleep);
            }
        }

        private void ProcessNextQueueItem()
        {
            if (IncommingInstructions.Count > 0)
            {
                Debug($"Processing next instruction...: {IncommingInstructions[0].Key}", DebugParams);

                LibraryExec(ParseMessage(IncommingInstructions[0].Key), IncommingInstructions[0].Value);
                IncommingInstructions.RemoveAt(0);

                Start();
            }
        }

        #endregion

        //-------------------------------------------------------------------------------------------------------------------
        //-         SENDING (EXTENDS MESSAGING)                                                                             -
        //-------------------------------------------------------------------------------------------------------------------

        #region -=[- SENDING -]=-

        public void EnableSending()
        {
            CommandSendingThread = new Thread(SendInstructionLoop);
            CommandSendingThread.Start();
        }

        private void SendInstructionLoop()
        {
            while (true)
            {
                if (OutgoingInstructions.Count > 0) SendNextQueueItem();
                Thread.Sleep(ThreadSleep);
            }
        }

        public void SendNextQueueItem()
        {
            if(OutgoingInstructions.Count > 0 && SendStreamOpen)
            {
                SendStreamOpen = false;
                Debug($"Start sending message to {OutgoingInstructions[0].Value.GetHashCode()}: [{OutgoingInstructions[0].Key}]", DebugParams);

                Data = Encoding.ASCII.GetBytes(EncodeMessage(OutgoingInstructions[0].Key));
                OutgoingInstructions[0].Value.BeginSend(Data, 0, Data.Length, SocketFlags.None, new AsyncCallback(OnClientSend), OutgoingInstructions[0].Value);
                Start();

                OutgoingInstructions.RemoveAt(0);
            }
        }

        #endregion

        //-------------------------------------------------------------------------------------------------------------------
        //-         MESSAGING                                                                                               -
        //-------------------------------------------------------------------------------------------------------------------

        #region -=[- MESSAGING -]=-

        /// <summary>
        /// Sends a message to a given client
        /// </summary>
        /// <param name="pMessage">Raw-Message to be sent</param>
        /// <param name="pSocket">Client-Socket</param>
        public void SendToClient(string pMessage, Socket pSocket)
        {
            if (pSocket != null && LClients.Authenticated(pSocket))
            {
                Debug($"Queueing message for {pSocket.GetHashCode()}: {pMessage}", DebugParams);
                OutgoingInstructions.Add(pMessage, pSocket);
            }
        }

        /// <summary>
        /// Sends a message to a given client
        /// </summary>
        /// <param name="pMessage">Raw-Message to be sent</param>
        /// <param name="pSocketHash">Hash of the clients socket</param>
        public void SendToClient(string pMessage, long pSocketHash) => SendToClient(pMessage, LClients[pSocketHash].Socket);

        /// <summary>
        /// Sends a message to a given client
        /// </summary>
        /// <param name="pMessage">Raw-Message to be sent</param>
        /// <param name="pIndex">Index of the client</param>
        public void SendToClient(string pMessage, int pIndex)
        {
            if(LClients[pIndex] != null)
            {
                Debug("Start Sending...", DebugParams);
                SendToClient(pMessage, LClients[pIndex].Socket);

            }
               
        }

        /// <summary>
        /// Sends a message to a given client
        /// </summary>
        /// <param name="pMessage">Raw-Message to be sent</param>
        /// <param name="pUsername">Username or Identifier of the clients socket</param>
        public void SendToClient(string pMessage, string pUsername) => SendToClient(pMessage, LClients[pUsername].Socket);

        /// <summary>
        /// Broadcasts a message to all connected clients
        /// </summary>
        /// <param name="pMessage">Raw-Message to be sent</param>
        public void Broadcast(string pMessage)
        {
            for(int i = 0; i < LClients.Count; i++)
                SendToClient(pMessage, LClients[i].Socket);
        }

        #endregion

        #endregion

        //===================================================================================================================
        //===================================================================================================================
        //=         CALLBACK METHODS                                                                                        =
        //===================================================================================================================
        //===================================================================================================================

        #region -=[- CALLBACK METHODS -]=-

        /// <summary>
        /// Gets called when a new client connects.
        /// </summary>
        /// <param name="AR"></param>
        private void OnClientAccept(IAsyncResult AR)
        {
            Debug($"New client connected!", DebugParams);

            // Accept the client socket and add it to the socket-list
            Socket clientSocket = ServerSocket.EndAccept(AR);
            LClients.Add(new NetComClientData(clientSocket));
            clientSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(OnClientReceive), clientSocket);
            
            // Re-Open the server-socket for further connections
            Start();
        }

        /// <summary>
        /// Gets called when a message is received
        /// </summary>
        /// <param name="AR"></param>
        private void OnClientReceive(IAsyncResult AR)
        {
            Socket clientSocket = (Socket)AR.AsyncState;
            int received = clientSocket.EndReceive(AR);
            byte[] tmpBuffer = new byte[received];
            Array.Copy(Buffer, tmpBuffer, received);

            Debug($"Incomming instruction from {clientSocket.GetHashCode()}: [{Encoding.ASCII.GetString(tmpBuffer)}]", DebugParams);

            IncommingInstructions.Add(Encoding.ASCII.GetString(tmpBuffer), clientSocket);

            Start();
        }

        /// <summary>
        /// Is called when the sending-process is done.
        /// </summary>
        /// <param name="AR"></param>
        private void OnClientSend(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;

            Debug($"Received Callback from {socket.GetHashCode()}. Ending transmission.", DebugParams);

            socket.EndSend(AR);
            SendStreamOpen = true;
        }

        #endregion
    }

#pragma warning restore 0168
}
