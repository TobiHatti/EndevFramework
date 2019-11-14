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
    /// Acting as a client.
    /// </summary>
    public class NetComClient : NetComOperator
    {
        private volatile Thread instructionReceptionThread = null;
        private string serverPublicKey = null;

        /// <summary>
        /// Creates a new NetCom-client instance.
        /// </summary>
        /// <param name="pServerIP">IP of the server</param>
        /// <param name="pPort">TCP port of the server</param>
        public NetComClient(string pServerIP, int pPort)
        {
            RSAKeys = RSAHandler.GenerateKeyPair();
            port = pPort;
            serverIP = IPAddress.Parse(pServerIP);


        }

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
            Debug("Setting up client...");
            LocalSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            Debug("Connecting to Server...");

            TryConnect();

            Debug("Starting Background-Process: Instruction-Receiving...");
            instructionReceptionThread = new Thread(AsyncInstructionReceptionLoop);
            instructionReceptionThread.Start();

            base.Start();

            Debug("Client setup complete!");
        }

        /// <summary>
        /// Tries to connect to the server.
        /// Returns if it was successfull.
        /// </summary>
        private void TryConnect()
        {
            int attempts = 0;
            while (!LocalSocket.Connected)
            {
                try
                {
                    Debug("Connection attempt " + attempts++);
                    LocalSocket.Connect(serverIP, port);
                }
                catch (SocketException) { }
            }

            Debug($"Connection successfull! Required {attempts} attempts");
        }

        /// <summary>
        /// Sends an instruction to the server.
        /// </summary>
        /// <param name="pInstruction">Instruction to be sent. Set the receiver-parameter to 'null'</param>
        public void Send(InstructionBase pInstruction)
        {
            pInstruction.SetReceiverPublicKey(serverPublicKey);

            outgoingInstructions.Add(pInstruction);
        }

        /// <summary>
        /// Loop for executing the AsyncInstructionReceiveNext-Method.
        /// </summary>
        protected void AsyncInstructionReceptionLoop()
        {
            while (true) AsyncInstructionReceiveNext();
        }

        /// <summary>
        /// Sends the next instruction from the outgoing-queue.
        /// </summary>
        protected override void AsyncInstructionSendNext()
        {
            byte[] buffer;

            buffer = Encoding.UTF8.GetBytes(outgoingInstructions[0].Encode());

            LocalSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);

            Debug($"Sent Message to {outgoingInstructions[0].Receiver.ToString()}.");
            Debug(outgoingInstructions[0].ToString());

            outgoingInstructions.RemoveAt(0);
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

            Debug("Received Message.");
            try
            {
                InstructionBase[] instructionList = InstructionOperations.Parse(this, null, text).ToArray();
                foreach (InstructionBase instr in instructionList)
                    incommingInstructions.Add(instr);
            }
            catch (NetComAuthenticationException)
            {
                Debug("Authentication-Error.");
            }
            catch (Exception)
            {
                Debug($"Error occured. ({errorCtr})");
                errorCtr++;
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
    }
}
