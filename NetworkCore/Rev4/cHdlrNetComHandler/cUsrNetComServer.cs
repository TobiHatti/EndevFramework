using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static EndevFramework.NetworkCore.NetComExceptions;

namespace EndevFramework.NetworkCore
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
                if (HandlerData.IncommingInstructions.Count > 0) return HandlerData.IncommingInstructions[0].Sender;
                else return null;
            }
        }


        /// <summary>
        /// Creates a new instance of the NetCom-Server on a given port.
        /// </summary>
        public NetComServer(NetComHandler pHandler, HandlerData pHandlerData) : base(pHandler, pHandlerData) { }



        /// <summary>
        /// Sends the next instruction from the outgoing-queue.
        /// </summary>
        protected override void AsyncInstructionSendNext()
        {
            base.AsyncInstructionSendNext();

            if (!haltActive)
            {
                try
                {
                    if ((HandlerData.OutgoingInstructions[0].Receiver as NetComCData).Authenticated
                        || HandlerData.OutgoingInstructions[0].GetType() != typeof(InstructionLibraryEssentials.KeyExchangeServer2Client)
                        || HandlerData.OutgoingInstructions[0].GetType() != typeof(InstructionLibraryEssentials.AuthenticationServer2Client))
                    {
                        Socket current = HandlerData.OutgoingInstructions[0]?.Receiver.LocalSocket;
                        byte[] data;

                        data = Encoding.UTF8.GetBytes(HandlerData.OutgoingInstructions[0].Encode());

                        try
                        {
                            current.Send(data);
                        }
                        catch (Exception)
                        {
                            Handler.Debug("Client disconnected > Connection lost. (101)", DebugType.Warning);
                            current.Close();
                            UserGroups.Disconnect(current);
                            ConnectedClients.Remove(current);
                            return;
                        }

                        Handler.Debug($"Sent Message to {HandlerData.OutgoingInstructions[0].Receiver.ToString()}.", DebugType.Info);
                        Handler.Debug(HandlerData.OutgoingInstructions[0].ToString(), DebugType.Info);
                        HandlerData.OutgoingInstructions.RemoveAt(0);
                    }
                    else
                    {
                        Handler.Debug($"Could not send Message to {HandlerData.OutgoingInstructions[0].Receiver.ToString()}. Authentication not valid.", DebugType.Error);
                        Handler.Debug($"Sending Authentication-Reminder...", DebugType.Info);

                        // Queues a authentication-reminder
                        Send(new InstructionLibraryEssentials.AuthenticationReminder(this, HandlerData.OutgoingInstructions[0].Receiver));

                        // moves the current instruction to the back
                        HandlerData.OutgoingInstructions.Add(HandlerData.OutgoingInstructions[0].Clone());
                        HandlerData.OutgoingInstructions.RemoveAt(0);
                    }


                }
                catch (Exception ex)
                {
                    Handler.Debug("Halting (08)", DebugType.Warning);
                    if (HandlerData.ShowExceptions) Handler.Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                    if (HandlerData.TryRestartOnCrash) HaltAllThreads();
                }
            }
            else
            {
                Handler.Debug("Could not send message. Server is in halt-mode. Waiting for 5 seconds...", DebugType.Error);
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
                Handler.Debug("Setting up server...", DebugType.Info);
                LocalSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                LocalSocket.Bind(new IPEndPoint(IPAddress.Any, HandlerData.Port));
                LocalSocket.Listen(0);
                LocalSocket.BeginAccept(AcceptCallback, null);

                base.Start();

                Handler.Debug("Server setup complete!", DebugType.Info);
            }
            catch (Exception ex)
            {
                Handler.Debug("Halting (07)", DebugType.Warning);
                if (HandlerData.ShowExceptions) Handler.Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                if (HandlerData.TryRestartOnCrash) HaltAllThreads();
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
                    Handler.Debug("Shutting down all connections...", DebugType.Info);
                    foreach (NetComCData client in ConnectedClients)
                    {
                        try
                        {
                            client.LocalSocket.Shutdown(SocketShutdown.Both);
                            client.LocalSocket.Close();
                        }
                        catch (Exception ex)
                        {
                            Handler.Debug("Could not close clients socket.", DebugType.Error);
                            if (HandlerData.ShowExceptions) Handler.Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                        }
                    }
                }

                Handler.Debug("Shutting down server...", DebugType.Info);
                LocalSocket.Close();
                Handler.Debug("Shutdown complete!", DebugType.Info);
            }
            catch (Exception ex)
            {
                Handler.Debug("Shutdown could not be completed. Some connections might stay opened.", DebugType.Error);
                if (HandlerData.ShowExceptions) Handler.Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
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
                Handler.Debug("New client connected.", DebugType.Info);

                //SendToClient(socket, new NCILib.PreAuth(this));
                Send(new InstructionLibraryEssentials.KeyExchangeServer2Client(this, ConnectedClients[socket]));

                LocalSocket.BeginAccept(AcceptCallback, null);
            }
            catch (Exception ex)
            {
                Handler.Debug("Halting (06)", DebugType.Warning);
                if (HandlerData.ShowExceptions) Handler.Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                if (HandlerData.TryRestartOnCrash) HaltAllThreads();
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
                    Handler.Debug("Client disconnected > Connection lost. (102)", DebugType.Warning);
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
                Handler.Debug("Received message.", DebugType.Info);
                Console.ForegroundColor = ConsoleColor.White;

                InstructionBase[] instructionList = null;

                try
                {
                    instructionList = InstructionOperations.Parse(this, current, text, ConnectedClients).ToArray();

                    // Check for group-Addignment
                    foreach (InstructionBase instr in instructionList)
                    {
                        HandlerData.IncommingInstructions.Add(instr);

                        if (instr.GetType() != typeof(InstructionLibraryEssentials.KeyExchangeClient2Server) && !groupAddRecords.Contains(instr.Sender.Username))
                        {
                            groupAddRecords.Add(instr.Sender.Username);
                            UserGroups.TryGroupAdd(instr.Sender);
                        }
                    }


                }
                catch (NetComAuthenticationException ex)
                {
                    Handler.Debug("Authentication-Error (Instruction-Parsing).", DebugType.Error);
                    if (HandlerData.ShowExceptions) Handler.Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                }
                catch (Exception ex)
                {
                    Handler.Debug($"Error occured (Instruction-Parsing). ({HandlerData.LogErrorCounter})", DebugType.Error);
                    if (HandlerData.ShowExceptions) Handler.Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                    HandlerData.LogErrorCounter++;
                }



                try
                {
                    current.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, current);
                }
                catch (SocketException)
                {
                    Handler.Debug("Client disconnected > Connection lost. (103)", DebugType.Warning);
                    // Don't shutdown because the socket may be disposed and its disconnected anyway.
                    current.Close();
                    UserGroups.Disconnect(current);
                    ConnectedClients.Remove(current);
                    return;
                }
            }
            catch (Exception ex)
            {
                Handler.Debug("Halting (05)", DebugType.Warning);
                if (HandlerData.ShowExceptions) Handler.Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                if (HandlerData.TryRestartOnCrash) HaltAllThreads();
            }
        }

        /// <summary>
        /// Sends an instruction to a single client.
        /// </summary>
        /// <param name="pInstruction">Instruction to send. The receiver is set by the 'pReceiver'-parameter</param>
        public void Send(InstructionBase pInstruction)
        {
            if (!haltActive)
            {
                if (pInstruction.Receiver != null)
                {
                    Handler.Debug($"Queueing message for {pInstruction.Receiver.ToString()}.", DebugType.Info);
                    HandlerData.OutgoingInstructions.Add(pInstruction);
                }
            }
        }

        /// <summary>
        /// Sends an instruction to all connected users.
        /// </summary>
        /// <param name="pInstruction">Instruction to send. Set the receiver-parameter to 'null'</param>
        public void Broadcast(InstructionBase pInstruction)
        {
            if (!haltActive)
            {
                try
                {
                    lock (ConnectedClients)
                    {
                        for (int i = 0; i < ConnectedClients.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(ConnectedClients[i].Username) && !string.IsNullOrEmpty(ConnectedClients[i].Password))
                            {
                                try
                                {
                                    InstructionBase tmpInstruction = pInstruction.Clone();
                                    tmpInstruction.Receiver = ConnectedClients[i];

                                    Handler.Debug($"Queueing message for {tmpInstruction.Receiver.ToString()}.", DebugType.Info);
                                    HandlerData.OutgoingInstructions.Add(tmpInstruction);
                                }
                                catch (Exception ex)
                                {
                                    Handler.Debug("Broadcast-Error.", DebugType.Error);
                                    if (HandlerData.ShowExceptions) Handler.Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                                    HandlerData.LogErrorCounter++;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Handler.Debug("Halting (04)", DebugType.Warning);
                    if (HandlerData.ShowExceptions) Handler.Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                    if (HandlerData.TryRestartOnCrash) HaltAllThreads();
                }
            }
        }

        /// <summary>
        /// Sends an instruction to a range of users.
        /// </summary>
        /// <param name="pInstruction">Instruction to send. Set the receiver-parameter to 'null'</param>
        /// <param name="pUsers">Target users</param>
        public void ListSend(InstructionBase pInstruction, params NetComUser[] pUsers)
        {
            if (!haltActive)
            {
                try
                {
                    for (int i = 0; i < pUsers.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(pUsers[i].Username) && !string.IsNullOrEmpty(pUsers[i].Password))
                        {
                            try
                            {
                                InstructionBase tmpInstruction = pInstruction.Clone();
                                tmpInstruction.Receiver = pUsers[i];

                                Handler.Debug($"Queueing message for {tmpInstruction.Receiver.ToString()}.", DebugType.Info);
                                HandlerData.OutgoingInstructions.Add(tmpInstruction);
                            }
                            catch (Exception ex)
                            {
                                Handler.Debug("ListSend-Error.", DebugType.Error);
                                if (HandlerData.ShowExceptions) Handler.Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                                HandlerData.LogErrorCounter++;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Handler.Debug("Halting (03)", DebugType.Warning);
                    if (HandlerData.ShowExceptions) Handler.Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                    if (HandlerData.TryRestartOnCrash) HaltAllThreads();
                }
            }
        }

        /// <summary>
        /// Sends a instruction to a user-group.
        /// </summary>
        /// <param name="pInstruction">Instruction to send. Set the receiver-parameter to 'null'</param>
        /// <param name="pGroup">Target group</param>
        public void GroupSend(InstructionBase pInstruction, UserGroup pGroup)
        {
            if (!haltActive)
            {
                try
                {
                    lock (pGroup)
                    {
                        for (int i = 0; i < pGroup.OnlineMembers.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(pGroup.OnlineMembers[i].Username) && !string.IsNullOrEmpty(pGroup.OnlineMembers[i].Password))
                            {
                                try
                                {
                                    InstructionBase tmpInstruction = pInstruction.Clone();
                                    tmpInstruction.Receiver = pGroup.OnlineMembers[i];

                                    Handler.Debug($"Queueing message for {tmpInstruction.Receiver.ToString()}.", DebugType.Info);
                                    HandlerData.OutgoingInstructions.Add(tmpInstruction);
                                }
                                catch (Exception ex)
                                {
                                    Handler.Debug("GroupSend-Error.", DebugType.Error);
                                    if (HandlerData.ShowExceptions) Handler.Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                                    HandlerData.LogErrorCounter++;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Handler.Debug("Halting (02)", DebugType.Warning);
                    if (HandlerData.ShowExceptions) Handler.Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                    if (HandlerData.TryRestartOnCrash) HaltAllThreads();
                }
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

                Handler.Debug("Shutting down all connections...", DebugType.Info);
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
                Handler.Debug("Attempting to restart server...", DebugType.Info);

                Handler.Debug("Redefining system-data...", DebugType.Info);
                groupAddRecords = new List<string>();
                ConnectedClients = new ClientList();
                UserGroups = new NetComGroups();

                base.RestartSystem();

                Start();

                Handler.Debug("Server restart complete!", DebugType.Info);
            }
            catch (Exception ex)
            {

                Handler.Debug("Halting (01)", DebugType.Warning);
                if (HandlerData.ShowExceptions) Handler.Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                if (HandlerData.TryRestartOnCrash) HaltAllThreads();
            }
        }











        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 1a │ F I E L D S   ( P R I V A T E )                        ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝    

        #region ═╣ F I E L D S   ( P R I V A T E ) ╠═ 
        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 1b │ F I E L D S   ( P R O T E C T E D )                    ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝    

        #region ═╣ F I E L D S   ( P R O T E C T E D ) ╠═ 
        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 1c │ D E L E G A T E S                                      ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝    

        #region ═╣ D E L E G A T E S ╠═ 
        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 2a │ P R O P E R T I E S   ( I N T E R N A L )              ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝  

        #region ═╣ P R O P E R T I E S   ( I N T E R N A L ) ╠═ 
        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 2b │ P R O P E R T I E S   ( P U B L I C )                  ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝  

        #region ═╣ P R O P E R T I E S   ( P U B L I C ) ╠═ 
        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 3  │ C O N S T R U C T O R S                                ║
        // ╚════╧════════════════════════════════════════════════════════╝  

        #region ═╣ C O N S T R U C T O R S ╠═ 
        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 4a │ M E T H O D S   ( P R I V A T E )                      ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝  

        #region ═╣ M E T H O D S   ( P R I V A T E ) ╠═ 
        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 4b │ M E T H O D S   ( P R O T E C T E D )                  ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝ 

        #region ═╣ M E T H O D S   ( P R O T E C T E D ) ╠═ 
        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 4c │ M E T H O D S   ( I N T E R N A L )                    ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝ 

        #region ═╣ M E T H O D S   ( I N T E R N A L ) ╠═ 
        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 4d │ M E T H O D S   ( P U B L I C )                        ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝ 

        #region ═╣ M E T H O D S   ( P U B L I C ) ╠═ 
        #endregion
    }
}
