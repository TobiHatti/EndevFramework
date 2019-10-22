using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EndevFWNetCore
{
#pragma warning disable 0168
    public class NetComServer
    {
        //===================================================================================================================
        //===================================================================================================================
        //=         PROPERTIES                                                                                              =
        //===================================================================================================================
        //===================================================================================================================

        #region -=[- FIELDS -]=-

        private const int BufferSize = 1048576; // 1 MB

        private int Port = 2225;
        private Socket serverSocket = null;

        private Thread CommandProcessingThread = null;
        private Thread CommandSendingThread = null;

        private byte[] Buffer = new byte[BufferSize];

  
        #endregion

        #region -=[- PROPERTIES -]=-

        public int ThreadSleep { get; set; } = 0;

        public NetComInstructionQueue IncommingInstructions { get; private set; } = new NetComInstructionQueue();
        public NetComInstructionQueue OutgoingInstructions { get; private set; } = new NetComInstructionQueue();
        public NetComClientList LClientList { get; private set; } = new NetComClientList();

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
        //=         CONSTRUCTORS                                                                                            =
        //===================================================================================================================
        //===================================================================================================================

        #region -=[- CONSTRUCTORS -]=-

        /// <summary>
        /// Creates a new TCP-Communication Server Object
        /// </summary>
        /// <param name="pPort">TCP-Port</param>
        public NetComServer(int pPort)
        {
            Port = pPort;
        }

        #endregion

        //===================================================================================================================
        //===================================================================================================================
        //=         CALLABLE METHODS                                                                                        =
        //===================================================================================================================
        //===================================================================================================================

        #region -=[- CALLABLE METHODS -]=-

        #region -=[- SERVER STATE -]=-

        /// <summary>
        /// Initializes and Starts the Server
        /// </summary>
        public void Start()
        {
            Debug("Setting up server...", DebugParams);

            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, Port));
            serverSocket.Listen(0);
            serverSocket.BeginAccept(AcceptCallback, null);

            Debug("Server setup complete!", DebugParams);

            CommandProcessingThread = new Thread(InstructionProcessingLoop);
            CommandProcessingThread.Start();

            CommandSendingThread = new Thread(InstructionSendingLoop);
            CommandSendingThread.Start();
        }

        /// <summary>
        /// Properly closes all connections 
        /// and shuts down the server
        /// </summary>
        public void Shutdown()
        {
            Debug("Shutting down all connections...", DebugParams);
            foreach (NetComClientListElement client in LClientList)
            {
                client.Socket.Shutdown(SocketShutdown.Both);
                client.Socket.Close();
            }

            Debug("Shutting down server...", DebugParams);
            serverSocket.Close();
            Debug("Shutdown complete!", DebugParams);
        }

        #endregion

        private void InstructionProcessingLoop()
        {
            while(true)
            {
                ProcessNextInstruction();
                Thread.Sleep(ThreadSleep);
            }
        }

        private void ProcessNextInstruction()
        {
            // TODO
            // Check if User is Authenticated
            // Else Reply to Client with the same message as sent, 
            // and the error that the user is not authenticated
        }

        private void InstructionSendingLoop()
        {
            while(true)
            {
                SendNextInstruction();
                Thread.Sleep(ThreadSleep);
            }
        }

        private void SendNextInstruction()
        {
            if (OutgoingInstructions.Count > 0)
            {
                try
                {
                    Socket current = OutgoingInstructions[0].Client.Socket;
                    byte[] data = Encoding.UTF8.GetBytes(OutgoingInstructions[0].Instruction);
                    current.Send(data);

                    Debug($"Sent Message to {OutgoingInstructions[0].Client.Username}: {OutgoingInstructions[0].Instruction}.", DebugParams);

                    OutgoingInstructions.RemoveAt(0);
                }
                catch 
                {
                    Debug($"An error occured whilst trying to send a message.", DebugParams);
                }
            }
        }

        #endregion

        //-------------------------------------------------------------------------------------------------------------------
        //-         SENDING                                                                                                 -
        //-------------------------------------------------------------------------------------------------------------------

        public void SendToClient(Socket pSocket, string pMessage) => SendToClient(LClientList[pSocket], pMessage);
        public void SendToClient(string pUsername, string pMessage) => SendToClient(LClientList[pUsername], pMessage);
        public void SendToClient(int pIndex, string pMessage) => SendToClient(LClientList[pIndex], pMessage);
        public void SendToClient(NetComClientListElement pClient, string pMessage)
        {
            if (pClient != null)
            {
                Debug($"Queueing message for {pClient.Username}: {pMessage}", DebugParams);
                OutgoingInstructions.Add(pMessage, pClient);
            }
        }

        public void Broadcast(string pMessage)
        {
            foreach(NetComClientListElement client in LClientList)
            {
                Debug($"Queueing message for {client.Username}: {pMessage}", DebugParams);
                OutgoingInstructions.Add(pMessage, client);
            }
        }

        //===================================================================================================================
        //===================================================================================================================
        //=         CALLBACK METHODS                                                                                        =
        //===================================================================================================================
        //===================================================================================================================

        #region -=[- CALLBACK METHODS -]=-

        /// <summary>
        /// Gets called when a connection is established
        /// </summary>
        /// <param name="AR">IAsyncResult</param>
        private void AcceptCallback(IAsyncResult AR)
        {
            Socket socket;

            try
            {
                socket = serverSocket.EndAccept(AR);
            }
            catch (ObjectDisposedException)
            {
                return;
            }

            LClientList.Add(socket);
            socket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, ReceiveCallback, socket);
            Debug("New client connected!", DebugParams);
            serverSocket.BeginAccept(AcceptCallback, null);
        }

        /// <summary>
        /// Gets called when a message is received
        /// </summary>
        /// <param name="AR">IAsyncResult</param>
        private void ReceiveCallback(IAsyncResult AR)
        {
            Socket current = (Socket)AR.AsyncState;
            int received;

            try
            {
                received = current.EndReceive(AR);
            }
            catch (SocketException)
            {
                Debug("Client forcefully disconnected.", DebugParams);
                // Don't shutdown because the socket may be disposed and its disconnected anyway.
                current.Close();
                LClientList.RemoveAt(current);
                return;
            }

            byte[] recBuf = new byte[received];
            Array.Copy(Buffer, recBuf, received);
            string text = Encoding.UTF8.GetString(recBuf);

            Debug("Received message: " + text, DebugParams);

            IncommingInstructions.Add(text, LClientList[current]);

            current.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, ReceiveCallback, current);
        }

        #endregion
    }
#pragma warning restore 0168
}
