using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NCILib = EndevFWNetCore.NetComInstructionLib;
using NCI = EndevFWNetCore.NetComInstruction;

namespace EndevFWNetCore
{
#pragma warning disable 0168
    public class NetComServer : NetComUser
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

        public int ThreadSleep { get; set; } = 100;

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
        public delegate string MessageParser(string pMessage, NetComClientData pClient);
        public MessageParser ParseMessage { get; set; } = null;

        // Message Encoder
        public delegate string MessageEncoder(string pMessage, NetComClientData pClient);
        public MessageEncoder EncodeMessage { get; set; } = null;   

        // Authentication lookup
        public delegate bool AuthenticationLookup(string pUsername, string pPassword);
        public AuthenticationLookup AuthLookup { get; set; } = null;

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

            NetComUser.LocalUser = this;
        }

        #endregion

        //===================================================================================================================
        //===================================================================================================================
        //=         CALLABLE METHODS                                                                                        =
        //===================================================================================================================
        //===================================================================================================================

        #region -=[- PUBLIC METHODS -]=-

        //-------------------------------------------------------------------------------------------------------------------
        //-         SERVER STATE                                                                                            -
        //-------------------------------------------------------------------------------------------------------------------

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
            foreach (NetComClientData client in LClientList)
            {
                client.Socket.Shutdown(SocketShutdown.Both);
                client.Socket.Close();
            }

            Debug("Shutting down server...", DebugParams);
            serverSocket.Close();
            Debug("Shutdown complete!", DebugParams);
        }

        #endregion

        //-------------------------------------------------------------------------------------------------------------------
        //-         SENDING (CLEAR / UNENCRYPTED)                                                                           -
        //-------------------------------------------------------------------------------------------------------------------

        #region -=[- SENDING UNENCRYPTED -]=-
        public void SendToClient(Socket pSocket, NetComInstruction pInstruction) => SendToClient(LClientList[pSocket], pInstruction);
        public void SendToClient(string pUsername, NetComInstruction pInstruction) => SendToClient(LClientList[pUsername], pInstruction);
        public void SendToClient(int pIndex, NetComInstruction pInstruction) => SendToClient(LClientList[pIndex], pInstruction);
        public void SendToClient(NetComClientData pClient, NetComInstruction pInstruction)
        {
            if (pClient != null)
            {
                Debug($"Queueing message for {pClient.Username}: {pInstruction}", DebugParams);
                OutgoingInstructions.Add(pInstruction, pClient);
            }
        }

        public void Broadcast(NetComInstruction pInstruction)
        {
            foreach (NetComClientData client in LClientList)
            {
                Debug($"Queueing message for {client.Username}: {pInstruction}", DebugParams);
                OutgoingInstructions.Add(pInstruction, client);
            }
        }

        #endregion

        //-------------------------------------------------------------------------------------------------------------------
        //-         SENDING (ENCRYPTED)                                                                                     -
        //-------------------------------------------------------------------------------------------------------------------

        #region -=[- SENDING ENCRYPTED -]=-

        public void SendToClientRSA(Socket pSocket, NetComInstruction pInstruction) => SendToClientRSA(LClientList[pSocket], pInstruction);
        public void SendToClientRSA(string pUsername, NetComInstruction pInstruction) => SendToClientRSA(LClientList[pUsername], pInstruction);
        public void SendToClientRSA(int pIndex, NetComInstruction pInstruction) => SendToClientRSA(LClientList[pIndex], pInstruction);
        public void SendToClientRSA(NetComClientData pClient, NetComInstruction pInstruction)
        {
            if (pClient != null)
            {
                Debug($"Queueing message for {pClient.Username}: {pInstruction}", DebugParams);
                OutgoingInstructions.AddRSA(pInstruction, pClient);
            }
        }

        public void BroadcastRSA(NetComInstruction pInstruction)
        {
            foreach (NetComClientData client in LClientList)
            {
                Debug($"Queueing message for {client.Username}: {pInstruction}", DebugParams);
                OutgoingInstructions.AddRSA(pInstruction, client);
            }
        }

        #endregion

        #endregion

        #region -=[- PRIVATE METHODS -]=-

        //-------------------------------------------------------------------------------------------------------------------
        //-         INCOMMING INSTRUCTIONS (PROCESSING)                                                                     -
        //-------------------------------------------------------------------------------------------------------------------

        #region -=[- INCOMMING INSTRUCTIONS -]=-

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
            if (IncommingInstructions.Count > 0)
            {
            
                IncommingInstructions[0]?.Instruction.Execute(IncommingInstructions[0]?.Client);
                IncommingInstructions.RemoveAt(0);
            }

            // TODO
            // Check if User is Authenticated
            // Else Reply to Client with the same message as sent, 
            // and the error that the user is not authenticated
        }

        #endregion

        //-------------------------------------------------------------------------------------------------------------------
        //-         OUTGOING INSTRUCTIONS (SENDING)                                                                         -
        //-------------------------------------------------------------------------------------------------------------------

        #region -=[- OUTGOING INSTRUCTIONS -]=-

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
          
                    Socket current = OutgoingInstructions[0]?.Client?.Socket;
                    byte[] data;


                    if (OutgoingInstructions[0].RSAEncrypted && OutgoingInstructions[0].Client.PublicKey != null)
                    {
                        data = Encoding.UTF8.GetBytes(OutgoingInstructions[0].Instruction.Encode(true, OutgoingInstructions[0].Client.PublicKey));
                    }
                    else
                    {
                        data = Encoding.UTF8.GetBytes(OutgoingInstructions[0].Instruction.Encode(false));
                    }

                    current.Send(data);

                    Debug($"Sent Message to {OutgoingInstructions[0].Client.Username}: {OutgoingInstructions[0].Instruction}.", DebugParams);

                    OutgoingInstructions.RemoveAt(0);
             
 
            }
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

            SendToClient(socket, new NCILib.PreAuth(this));

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

            Console.ForegroundColor = ConsoleColor.Cyan;
            Debug("Received message: " + text, DebugParams);
            Console.ForegroundColor = ConsoleColor.White;


            NetComInstruction[] instructionList = NetComInstruction.Parse(this, text).ToArray();

            foreach (NetComInstruction instr in instructionList)
                IncommingInstructions.Add(instr, LClientList[current]);

            current.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, ReceiveCallback, current);
        }

        #endregion
    }
#pragma warning restore 0168
}
