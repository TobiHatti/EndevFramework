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
            if (!haltActive)
            {
                try
                {
                    if ((OutgoingInstructions[0].Receiver as NetComCData).Authenticated
                        || OutgoingInstructions[0].GetType() != typeof(InstructionLibraryEssentials.KeyExchangeServer2Client)
                        || OutgoingInstructions[0].GetType() != typeof(InstructionLibraryEssentials.AuthenticationServer2Client))
                    {
                        Socket current = OutgoingInstructions[0]?.Receiver.LocalSocket;
                        byte[] data;

                        data = Encoding.UTF8.GetBytes(OutgoingInstructions[0].Encode());

                        try
                        {
                            current.Send(data);
                        }
                        catch (Exception)
                        {
                            Debug("Client disconnected > Connection lost.", DebugType.Warning);
                            current.Close();
                            UserGroups.Disconnect(current);
                            ConnectedClients.Remove(current);
                            return;
                        }

                        Debug($"Sent Message to {OutgoingInstructions[0].Receiver.ToString()}.", DebugType.Info);
                        Debug(OutgoingInstructions[0].ToString(), DebugType.Info);
                        OutgoingInstructions.RemoveAt(0);
                    }
                    else
                    {
                        Debug($"Could not send Message to {OutgoingInstructions[0].Receiver.ToString()}. Authentication not valid.", DebugType.Error);
                        Debug($"Sending Authentication-Reminder...", DebugType.Info);

                        // Queues a authentication-reminder
                        Send(new InstructionLibraryEssentials.AuthenticationReminder(this, OutgoingInstructions[0].Receiver));

                        // moves the current instruction to the back
                        OutgoingInstructions.Add(OutgoingInstructions[0].Clone());
                        OutgoingInstructions.RemoveAt(0);
                    }

                    
                }
                catch (Exception ex)
                {
                    Debug("Halting (08)", DebugType.Warning);
                    if (ShowExceptions) Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                    if (AutoRestartOnCrash) HaltAllThreads();
                }
            }
            else
            {
                Debug("Could not send message. Server is in halt-mode. Waiting for 5 seconds...", DebugType.Error);
                Thread.Sleep(5000);
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
                Debug("Setting up server...", DebugType.Info);
                LocalSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                LocalSocket.Bind(new IPEndPoint(IPAddress.Any, port));
                LocalSocket.Listen(0);
                LocalSocket.BeginAccept(AcceptCallback, null);

                base.Start();

                Debug("Server setup complete!", DebugType.Info);
            }
            catch (Exception ex)
            {
                Debug("Halting (07)", DebugType.Warning);
                if (ShowExceptions) Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                if (AutoRestartOnCrash) HaltAllThreads();
            }
        }

        /// <summary>
        /// Properly closes all connections 
        /// and shuts down the server.
        /// </summary>
        public void Shutdown()
        {
            try
            {
                lock (ConnectedClients)
                {
                    Debug("Shutting down all connections...", DebugType.Info);
                    foreach (NetComCData client in ConnectedClients)
                    {
                        try
                        {
                            client.LocalSocket.Shutdown(SocketShutdown.Both);
                            client.LocalSocket.Close();
                        }
                        catch (Exception ex)
                        {
                            Debug("Could not close clients socket.", DebugType.Error);
                            if (ShowExceptions) Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                        }
                    }
                }

                Debug("Shutting down server...", DebugType.Info);
                LocalSocket.Close();
                Debug("Shutdown complete!", DebugType.Info);
            }
            catch (Exception ex)
            {
                Debug("Shutdown could not be completed. Some connections might stay opened.", DebugType.Error);
                if (ShowExceptions) Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
            }

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
                Debug("New client connected.", DebugType.Info);

                //SendToClient(socket, new NCILib.PreAuth(this));
                Send(new InstructionLibraryEssentials.KeyExchangeServer2Client(this, ConnectedClients[socket]));

                LocalSocket.BeginAccept(AcceptCallback, null);
            }
            catch (Exception ex)
            {
                Debug("Halting (06)", DebugType.Warning);
                if (ShowExceptions) Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                if (AutoRestartOnCrash) HaltAllThreads();
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
                    Debug("Client disconnected > Connection lost.", DebugType.Warning);
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
                Debug("Received message.", DebugType.Info);
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
                catch (NetComAuthenticationException ex)
                {
                    Debug("Authentication-Error (Instruction-Parsing).", DebugType.Error);
                    if (ShowExceptions) Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                }
                catch (Exception ex)
                {
                    Debug($"Error occured (Instruction-Parsing). ({_logErrorCount})", DebugType.Error);
                    if (ShowExceptions) Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                    _logErrorCount++;
                }
                


                try
                {
                    current.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, current);
                }
                catch (SocketException)
                {
                    Debug("Client disconnected > Connection lost.", DebugType.Warning);
                    // Don't shutdown because the socket may be disposed and its disconnected anyway.
                    current.Close();
                    UserGroups.Disconnect(current);
                    ConnectedClients.Remove(current);
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug("Halting (05)", DebugType.Warning);
                if (ShowExceptions) Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                if (AutoRestartOnCrash) HaltAllThreads();
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
                Debug($"Queueing message for {pInstruction.Receiver.ToString()}.", DebugType.Info);
                OutgoingInstructions.Add(pInstruction);
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

                            Debug($"Queueing message for {tmpInstruction.Receiver.ToString()}.", DebugType.Info);
                            OutgoingInstructions.Add(tmpInstruction);
                        }
                        catch (Exception ex)
                        {
                            Debug("Broadcast-Error.", DebugType.Error);
                            if (ShowExceptions) Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                            _logErrorCount++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug("Halting (04)", DebugType.Warning);
                if (ShowExceptions) Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                if (AutoRestartOnCrash) HaltAllThreads();
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

                        Debug($"Queueing message for {tmpInstruction.Receiver.ToString()}.", DebugType.Info);
                        OutgoingInstructions.Add(tmpInstruction);
                    }
                    catch (Exception ex)
                    {
                        Debug("ListSend-Error.", DebugType.Error);
                        if (ShowExceptions) Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                        _logErrorCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug("Halting (03)", DebugType.Warning);
                if (ShowExceptions) Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                if (AutoRestartOnCrash) HaltAllThreads();
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

                            Debug($"Queueing message for {tmpInstruction.Receiver.ToString()}.", DebugType.Info);
                            OutgoingInstructions.Add(tmpInstruction);
                        }
                        catch (Exception ex)
                        {
                            Debug("GroupSend-Error.", DebugType.Error);
                            if (ShowExceptions) Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                            _logErrorCount++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug("Halting (02)", DebugType.Warning);
                if (ShowExceptions) Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                if (AutoRestartOnCrash) HaltAllThreads();
            }
        }

        /// <summary>
        /// Halts all threads and prepares for a restart.
        /// </summary>
        protected override void HaltAllThreads()
        {
            if (!haltActive)
            {
                base.HaltAllThreads();

                Debug("Shutting down all connections...", DebugType.Info);
                Shutdown();
            }
        }

        /// <summary>
        /// Restarts the system.
        /// </summary>
        protected override void RestartSystem()
        {
            try
            {
                Debug("Attempting to restart server...", DebugType.Info);

                Debug("Redefining system-data...", DebugType.Info);
                groupAddRecords = new List<string>();
                ConnectedClients = new ClientList();
                UserGroups = new NetComGroups();

                base.RestartSystem();

                Start();

                Debug("Server restart complete!", DebugType.Info);
            }
            catch (Exception ex)
            {

                Debug("Halting (01)", DebugType.Warning);
                if (ShowExceptions) Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                if (AutoRestartOnCrash) HaltAllThreads();
            }
        }
    }
}
