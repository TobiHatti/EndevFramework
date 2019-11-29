using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static EndevFrameworkNetworkCore.NetComExceptions;

namespace EndevFrameworkNetworkCore
{
    /// <summary>
    /// =====================================   <para />
    /// FRAMEWORK: EndevFrameworkNetworkCore    <para />
    /// SUB-PACKAGE: User-Objects               <para />
    /// =====================================   <para />
    /// DESCRIPTION:                            <para />
    /// Main Handler for Net-Communication.     <para />
    /// Acting as a client.
    /// </summary>
    public class NetComClient : NetComOperator
    {
        internal Thread InstructionReceptionThread { get; set; } = null;
        private string serverPublicKey = null;

        public NetComClient(NetComHandler pHandler, HandlerData pHandlerData) : base(pHandler, pHandlerData) { }

        /// <summary>
        /// Sets the username and password for authenticating at the server.
        /// </summary>
        /// <param name="pUsername">Clients username</param>
        /// <param name="pPassword">Clients password</param>
        public void Login(string pUsername, string pPassword)
        {
            Username = pUsername;
            Password = pPassword;
        }

        /// <summary>
        /// Starts all tasks required for the Client
        /// </summary>
        public override void Start()
        {
            try
            {
                Handler.Debug("Setting up client...", DebugType.Info);
                LocalSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                Handler.Debug("Connecting to Server...", DebugType.Info);

                TryConnect();

                Handler.Debug("Starting Background-Process: Instruction-Receiving...", DebugType.Info);
                InstructionReceptionThread = new Thread(AsyncInstructionReceptionLoop);
                InstructionReceptionThread.Start();

                base.Start();

                Handler.Debug("Client setup complete!", DebugType.Info);
            }
            catch (Exception ex)
            {
                Handler.Debug("Halting (14)", DebugType.Warning);
                if (HandlerData.ShowExceptions) Handler.Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);

                if (HandlerData.TryRestartOnCrash) HaltAllThreads();
            }
        }

        /// <summary>
        /// Tries to connect to the server.
        /// Returns if it was successfull.
        /// </summary>
        private void TryConnect()
        {
            try
            {
                int attempts = 0;
                while (!LocalSocket.Connected)
                {
                    try
                    {
                        Handler.Debug("Connection attempt " + attempts++, DebugType.Warning);
                        LocalSocket.Connect(HandlerData.ServerIP, HandlerData.Port);
                    }
                    catch (SocketException) { }
                }

                Handler.Debug($"Connection successfull! Required {attempts} attempts", DebugType.Info);
            }
            catch (Exception ex)
            {
                Handler.Debug("Halting (13)", DebugType.Warning);
                if (HandlerData.ShowExceptions) Handler.Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                if (HandlerData.TryRestartOnCrash) HaltAllThreads();
            }
        }

        /// <summary>
        /// Sends an instruction to the server.
        /// </summary>
        /// <param name="pInstruction">Instruction to be sent. Set the receiver-parameter to 'null'</param>
        public void Send(InstructionBase pInstruction)
        {
            try
            {
                pInstruction.SetReceiverPublicKey(serverPublicKey);

                HandlerData.OutgoingInstructions.Add(pInstruction);
            }
            catch (Exception ex)
            {
                Handler.Debug("Could not add instruction to send-queue.", DebugType.Error);
                if (HandlerData.ShowExceptions) Handler.Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
            }
        }

        /// <summary>
        /// Loop for executing the AsyncInstructionReceiveNext-Method.
        /// </summary>
        protected void AsyncInstructionReceptionLoop()
        {
            try
            {
                while (!HandlerData.HaltActive) AsyncInstructionReceiveNext();
            }
            catch (Exception ex)
            {
                Handler.Debug("Halting (12)", DebugType.Warning);
                if (HandlerData.ShowExceptions) Handler.Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                if (HandlerData.TryRestartOnCrash) HaltAllThreads();
            }

            return;
        }

        /// <summary>
        /// Sends the next instruction from the outgoing-queue.
        /// </summary>
        protected override void AsyncInstructionSendNext()
        {
            base.AsyncInstructionSendNext();

            if (!HandlerData.HaltActive)
            {
                byte[] buffer;

                buffer = Encoding.UTF8.GetBytes(HandlerData.OutgoingInstructions[0].Encode());

                LocalSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);

                Handler.Debug($"Sent Message to {HandlerData.OutgoingInstructions[0].Receiver.ToString()}.", DebugType.Info);
                Handler.Debug(HandlerData.OutgoingInstructions[0].ToString(), DebugType.Info);

                HandlerData.OutgoingInstructions.RemoveAt(0);
            }
            else
            {
                Handler.Debug("Could not send message. Client is in halt-mode. Waiting for 5 seconds...", DebugType.Error);
                Thread.Sleep(5000);
            }
        }

        /// <summary>
        /// Receives and adds the next incomming instruction to the incomming-queue.
        /// </summary>
        protected void AsyncInstructionReceiveNext()
        {
            buffer = new byte[bufferSize];

            int received = LocalSocket.Receive(buffer, SocketFlags.None);
            if (received == 0) return;

            byte[] data = new byte[received];
            Array.Copy(buffer, data, received);
            string text = Encoding.UTF8.GetString(data);

            Handler.Debug("Received Message.", DebugType.Info);
            try
            {
                InstructionBase[] instructionList = InstructionOperations.Parse(this, null, text).ToArray();
                foreach (InstructionBase instr in instructionList)
                    HandlerData.IncommingInstructions.Add(instr);
            }
            catch (NetComAuthenticationException ex)
            {
                Handler.Debug("Authentication-Error.", DebugType.Error);
                if (HandlerData.ShowExceptions) Handler.Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
            }
            catch (Exception ex)
            {
                Handler.Debug($"Error occured. ({HandlerData.LogErrorCounter})", DebugType.Error);
                if (HandlerData.ShowExceptions) Handler.Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                HandlerData.LogErrorCounter++;
            }
        }

        /// <summary>
        /// Sets the RSA-PublicKey of the server.
        /// </summary>
        /// <param name="pPublicRSAKey">Servers public key</param>
        internal void SetServerRSA(string pPublicRSAKey)
        {
            serverPublicKey = pPublicRSAKey;
        }

        /// <summary>
        /// Halts all threads and prepares for a restart
        /// </summary>
        protected override void HaltAllThreads()
        {
            if (!HandlerData.HaltActive)
            {
                base.HaltAllThreads();

                Handler.Debug("Halting Instruction-Reception...", DebugType.Fatal);
                try
                {
                    InstructionReceptionThread.Abort();
                    Handler.Debug("Successfully stopped Instruction-Reception!", DebugType.Fatal);
                }
                catch (Exception ex)
                {
                    Handler.Debug("Could not stop Instruction-Reception!", DebugType.Fatal);
                    if (HandlerData.ShowExceptions) Handler.Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                }
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
