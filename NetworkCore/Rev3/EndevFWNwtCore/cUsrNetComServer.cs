using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EndevFrameworkNetworkCore
{
    /// <summary>
    /// =====================================   <para />
    /// FRAMEWORK: EndevFrameworkNetworkCore    <para />
    /// SUB-PACKAGE: User-Objects               <para />
    /// =====================================   <para />
    /// DESCRIPTION:                            <para />
    /// Main Handler for Net-Communication.     <para />     
    /// Acting as a server.
    /// </summary>
    public class NetComServer : NetComOperator
    {
        private List<string> groupAddRecords = new List<string>();
        public ClientList ConnectedClients { get; private set; } = new ClientList();
        public NetComGroups UserGroups { get; private set; } = new NetComGroups();

        /// <summary>
        /// Gets the client of the currently processing instruction.
        /// </summary>
        internal NetComUser CurrentProcessingClient 
        { 
            get
            {
                if (incommingInstructions.Count > 0) return incommingInstructions[0].Sender;
                else return null;
            }
        }

        /// <summary>
        /// Creates a new instance of the NetCom-Server on a given port.
        /// </summary>
        /// <param name="pPort">Target TCP-Port</param>
        public NetComServer(int pPort)
        {
            port = pPort;
            serverIP = IPAddress.Any;

            RSAKeys = RSAHandler.GenerateKeyPair();
        }


        /// <summary>
        /// Sets the tool used for authenticating users.
        /// </summary>
        /// <param name="pLookupTool">Lookup-Method for user-authentication</param>
        public void SetAuthenticationTool(NetComCData.AuthenticationTool pLookupTool)
        {
            NetComCData.AuthLookup = pLookupTool;
        }

        /// <summary>
        /// Sends the next instruction from the outgoing-queue.
        /// </summary>
        protected override void AsyncInstructionSendNext()
        {
            try
            {
                Socket current = outgoingInstructions[0]?.Receiver.LocalSocket;
                byte[] data;

                data = Encoding.UTF8.GetBytes(outgoingInstructions[0].Encode());

                try
                {
                    current.Send(data);
                }
                catch
                {
                    Debug("Client disconnected > Connection lost.");
                    current.Close();
                    UserGroups.Disconnect(current);
                    ConnectedClients.Remove(current);
                    return;
                }

                Debug($"Sent Message to {outgoingInstructions[0].Receiver.ToString()}.");
                Debug(outgoingInstructions[0].ToString());

                outgoingInstructions.RemoveAt(0);
            }
            catch
            {
                if (AutoRestartOnCrash) RestartSystem();
            }
        }

        /// <summary>
        /// Executes tasks every few minutes. Used for cleanup, improvements, etc.
        /// </summary>
        protected override void AsyncLongTermNextCycle()
        {
            base.AsyncLongTermNextCycle();

            // Clear groupAddRecords to check every sending client once again 
            groupAddRecords.Clear();
        }

        /// <summary>
        /// Initializes and Starts the Server.
        /// </summary>
        public override void Start()
        {
            try
            {
                Debug("Setting up server...");
                LocalSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                LocalSocket.Bind(new IPEndPoint(IPAddress.Any, port));
                LocalSocket.Listen(0);
                LocalSocket.BeginAccept(AcceptCallback, null);

                base.Start();

                Debug("Server setup complete!");
            }
            catch
            {
                if (AutoRestartOnCrash) RestartSystem();
            }
        }

        /// <summary>
        /// Properly closes all connections 
        /// and shuts down the server.
        /// </summary>
        public void Shutdown()
        {
            lock (ConnectedClients)
            {
                Debug("Shutting down all connections...");
                foreach (NetComCData client in ConnectedClients)
                {
                    client.LocalSocket.Shutdown(SocketShutdown.Both);
                    client.LocalSocket.Close();
                }
            }

            Debug("Shutting down server...");
            LocalSocket.Close();
            Debug("Shutdown complete!");
        }

        /// <summary>
        /// Gets called when a connection is established.
        /// </summary>
        /// <param name="AR">IAsyncResult</param>
        private void AcceptCallback(IAsyncResult AR)
        {
            try
            {
                Socket socket;

                try
                {
                    socket = LocalSocket.EndAccept(AR);
                }
                catch (ObjectDisposedException)
                {
                    return;
                }

                ConnectedClients.Add(socket);

                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, socket);
                Debug("New client connected.");

                //SendToClient(socket, new NCILib.PreAuth(this));
                Send(new InstructionLibraryEssentials.KeyExchangeServer2Client(this, ConnectedClients[socket]));

                LocalSocket.BeginAccept(AcceptCallback, null);
            }
            catch
            {
                if (AutoRestartOnCrash) RestartSystem();
            }
        }

        /// <summary>
        /// Gets called when a message is received.
        /// </summary>
        /// <param name="AR">IAsyncResult</param>
        private void ReceiveCallback(IAsyncResult AR)
        {
            try
            {
                Socket current = (Socket)AR.AsyncState;
                int received;

                try
                {
                    received = current.EndReceive(AR);
                }
                catch (SocketException)
                {
                    Debug("Client disconnected > Connection lost.");
                    // Don't shutdown because the socket may be disposed and its disconnected anyway.
                    current.Close();
                    UserGroups.Disconnect(current);
                    ConnectedClients.Remove(current);
                    return;
                }

                byte[] recBuf = new byte[received];
                Array.Copy(buffer, recBuf, received);
                string text = Encoding.UTF8.GetString(recBuf);

                Console.ForegroundColor = ConsoleColor.Cyan;
                //Debug("Received message: " + text);
                Debug("Received message.");
                Console.ForegroundColor = ConsoleColor.White;

                InstructionBase[] instructionList = null;

                try
                {
                    instructionList = InstructionOperations.Parse(this, current, text, ConnectedClients).ToArray();

                    // Check for group-Addignment
                    foreach (InstructionBase instr in instructionList)
                    {
                        incommingInstructions.Add(instr);

                        if (instr.GetType() != typeof(InstructionLibraryEssentials.KeyExchangeClient2Server) && !groupAddRecords.Contains(instr.Sender.Username))
                        {
                            groupAddRecords.Add(instr.Sender.Username);
                            UserGroups.TryGroupAdd(instr.Sender);
                        }
                    }


                }
                catch (NetComAuthenticationException)
                {
                    Debug("Authentication-Error (Instruction-Parsing).");
                }
                catch (Exception)
                {
                    Debug($"Error occured (Instruction-Parsing). ({errorCtr})");
                    errorCtr++;
                }


                try
                {
                    current.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, current);
                }
                catch (SocketException)
                {
                    Debug("Client disconnected > Connection lost.");
                    // Don't shutdown because the socket may be disposed and its disconnected anyway.
                    current.Close();
                    UserGroups.Disconnect(current);
                    ConnectedClients.Remove(current);
                    return;
                }
            }
            catch
            {
                if (AutoRestartOnCrash) RestartSystem();
            }
        }

        /// <summary>
        /// Sends an instruction to a single client.
        /// </summary>
        /// <param name="pInstruction">Instruction to send. The receiver is set by the 'pReceiver'-parameter</param>
        public void Send(InstructionBase pInstruction)
        {
            if (pInstruction.Receiver != null)
            {
                Debug($"Queueing message for {pInstruction.Receiver.ToString()}.");
                outgoingInstructions.Add(pInstruction);
            }
        }

        /// <summary>
        /// Sends an instruction to all connected users.
        /// </summary>
        /// <param name="pInstruction">Instruction to send. Set the receiver-parameter to 'null'</param>
        public void Broadcast(InstructionBase pInstruction)
        {
            try
            {
                lock (ConnectedClients)
                {
                
                    for (int i = 0; i < ConnectedClients.Count; i++)
                    {
                        try
                        {
                            InstructionBase tmpInstruction = pInstruction.Clone();
                            tmpInstruction.Receiver = ConnectedClients[i];

                            Debug($"Queueing message for {tmpInstruction.Receiver.ToString()}.");
                            outgoingInstructions.Add(tmpInstruction);
                        }
                        catch (IndexOutOfRangeException)
                        {
                            Debug("Broadcast-Error.");
                            errorCtr++;
                        }
                    }
                }
            }
            catch
            {
                if (AutoRestartOnCrash) RestartSystem();
            }
        }

        /// <summary>
        /// Sends an instruction to a range of users.
        /// </summary>
        /// <param name="pInstruction">Instruction to send. Set the receiver-parameter to 'null'</param>
        /// <param name="pUsers">Target users</param>
        public void ListSend(InstructionBase pInstruction, params NetComUser[] pUsers)
        {
            try
            {
                for (int i = 0; i < pUsers.Length; i++)
                {
                    try
                    {
                        InstructionBase tmpInstruction = pInstruction.Clone();
                        tmpInstruction.Receiver = pUsers[i];

                        Debug($"Queueing message for {tmpInstruction.Receiver.ToString()}.");
                        outgoingInstructions.Add(tmpInstruction);
                    }
                    catch
                    {
                        Debug("ListSend-Error.");
                        errorCtr++;
                    }
                }
            }
            catch
            {
                if (AutoRestartOnCrash) RestartSystem();
            }
        }

        /// <summary>
        /// Sends a instruction to a user-group.
        /// </summary>
        /// <param name="pInstruction">Instruction to send. Set the receiver-parameter to 'null'</param>
        /// <param name="pGroup">Target group</param>
        public void GroupSend(InstructionBase pInstruction, UserGroup pGroup)
        {
            try
            {
                lock (pGroup)
                {
                    for (int i = 0; i < pGroup.OnlineMembers.Count; i++)
                    {
                        try
                        {
                            InstructionBase tmpInstruction = pInstruction.Clone();
                            tmpInstruction.Receiver = pGroup.OnlineMembers[i];

                            Debug($"Queueing message for {tmpInstruction.Receiver.ToString()}.");
                            outgoingInstructions.Add(tmpInstruction);
                        }
                        catch
                        {
                            Debug("GroupSend-Error.");
                            errorCtr++;
                        }
                    }
                }
            }
            catch
            {
                if (AutoRestartOnCrash) RestartSystem();
            }
        }

        protected override void RestartSystem()
        {
            try
            {
                Debug("A fatal error occured. Attempting to restart server...");

                Shutdown();

                groupAddRecords = new List<string>();
                ConnectedClients = new ClientList();
                UserGroups = new NetComGroups();

                base.RestartSystem();

                Start();

                Debug("Server restart complete!");
            }
            catch
            {
                RestartSystem();
            }
        }
    }
}
