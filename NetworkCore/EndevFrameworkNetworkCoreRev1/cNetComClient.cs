using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EndevFrameworkNetworkCoreRev1
{
#pragma warning disable 0168
    public class NetComClient
    {
        //===================================================================================================================
        //===================================================================================================================
        //=         PROPERTIES                                                                                              =
        //===================================================================================================================
        //===================================================================================================================

        #region -=[- PROPERTIES -]=-

        public const int BufferSize = 1048576; // 1 MB
        public Socket ClientSocket { get; private set; } = null;
        public int Port { get; private set; }
        public IPAddress ServerIP { get; private set; } = IPAddress.Parse("127.0.0.1");
        public Dictionary<int, Socket> LClientSockets { get; private set; } = new Dictionary<int, Socket>();
        private byte[] Buffer { get; set; } = new byte[BufferSize];    
        private byte[] Data { get; set; } = null;
        public NetComInstructionQueue<string, Socket> IncommingInstructions { get; private set; } = new NetComInstructionQueue<string, Socket>();
        public NetComInstructionQueue<string, Socket> OutgoingInstructions { get; set; } = new NetComInstructionQueue<string, Socket>();
        public bool Connected { get; private set; } = false;
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

        public NetComClient(string pServerIP, int pPort)
        {
            ServerIP = IPAddress.Parse(pServerIP);
            Port = pPort;

            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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

        public void Init()
        {
            if (Debug == null || ParseMessage == null || EncodeMessage == null || LibraryExec == null)
                throw new NetComException("Error: Every delegate must be set! (Debug, ParseMessage, EncodeMessage, LibraryExec)");
        }

        private Thread CommandListeningThread = null;

        public void Start()
        {
            if (!ClientSocket.Connected) Connect();
        }


        /// <summary>
        /// Tries to connect to the server
        /// </summary>
        public void Connect()
        {
            int maxConAtmpts = 15;
            int i = 0;
            Debug("Connecting to Server...", DebugParams);
            while (!ClientSocket.Connected)
            {
                try { ClientSocket.Connect(ServerIP, Port); }
                catch (SocketException)  { Debug($"Connection failed (Atmp. {i+1})", DebugParams); }

                if (++i >= maxConAtmpts)
                {
                    Debug($"Failed {maxConAtmpts} Attempts. Terminating.", DebugParams);
                    return;
                }
            }
            Debug("Connection successfull!", DebugParams);
        }

        #endregion

        //-------------------------------------------------------------------------------------------------------------------
        //-         LISTENING                                                                                               -
        //-------------------------------------------------------------------------------------------------------------------

        #region -=[- LISTENING -]=-

        public void EnableListening()
        {
            CommandListeningThread = new Thread(ListenCycle);
            CommandListeningThread.Start();
        }

        private void ListenCycle()
        {
            while (true)
            {
                Debug("Start Listening...", DebugParams);
                byte[] recBuffer = new byte[BufferSize];    
                int rec = ClientSocket.Receive(recBuffer);
                byte[] data = new byte[rec];
                Array.Copy(recBuffer, data, rec);

                IncommingInstructions.Add(Encoding.ASCII.GetString(data), null);

                Debug("Received: " + Encoding.ASCII.GetString(data), DebugParams);
            }
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
            if (OutgoingInstructions.Count > 0)
            {
                Debug($"Start sending message to Server: [{OutgoingInstructions[0].Key}]", DebugParams);

                Data = Encoding.ASCII.GetBytes(EncodeMessage(OutgoingInstructions[0].Key));

                ClientSocket.Send(Data);

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
        public void SendToServer(string pMessage)
        {
            Debug($"Queueing message for Server: [{pMessage}]", DebugParams);
            OutgoingInstructions.Add(pMessage, null); 
        }

        #endregion

        #endregion

        //===================================================================================================================
        //===================================================================================================================
        //=         CALLBACK METHODS                                                                                        =
        //===================================================================================================================
        //===================================================================================================================

        #region -=[- CALLBACK METHODS -]=-

        // Synchronus Operation - No callbacks needed?

        #endregion

    }
#pragma warning restore 0168
}
