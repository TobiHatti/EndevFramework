using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EndevFWNwtCore
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

        public void Login(string pUsername, string pPassword)
        {
            Username = pUsername;
            Password = pPassword;
        }

        public void Start()
        {
            Debug("Setting up client...");
            LocalSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Debug("Client setup complete!");

            TryConnect();

            Debug("Starting Background-Process: Instruction-Processing...");
            instructionProcessingThread = new Thread(AsyncInstructionProcessingLoop);
            instructionProcessingThread.Start();

            Debug("Starting Background-Process: Instruction-Sending...");
            instructionSendingThread = new Thread(AsyncInstructionSendingLoop);
            instructionSendingThread.Start();

            Debug("Starting Background-Process: Instruction-Receiving...");
            instructionReceptionThread = new Thread(AsyncInstructionReceptionLoop);
            instructionReceptionThread.Start();

            Debug("Successfully started all background-processes!");
        }

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

        public void Send(InstructionBase pInstruction)
        {
            pInstruction.SetReceiverPublicKey(serverPublicKey);

            outgoingInstructions.Add(pInstruction);
        }

        protected void AsyncInstructionReceptionLoop()
        {
            while (true) AsyncInstructionReceiveNext();
        }

        protected override void AsyncInstructionSendNext()
        {
            byte[] buffer;

            buffer = Encoding.UTF8.GetBytes(outgoingInstructions[0].Encode());

            LocalSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);

            Debug($"Sent Message to {outgoingInstructions[0].Receiver.ToString()}.");
            Debug(outgoingInstructions[0].ToString());

            outgoingInstructions.RemoveAt(0);
        }
        
        protected void AsyncInstructionReceiveNext()
        {
            buffer = new byte[bufferSize];

            int received = LocalSocket.Receive(buffer, SocketFlags.None);
            if (received == 0) return;

            byte[] data = new byte[received];
            Array.Copy(buffer, data, received);
            string text = Encoding.UTF8.GetString(data);

            Debug("Received Message.");

            InstructionBase[] instructionList = InstructionOperations.Parse(this, null, text).ToArray();

            foreach (InstructionBase instr in instructionList)
                incommingInstructions.Add(instr);
        }

        public void SetServerRSA(string pPublicRSAKey)
        {
            serverPublicKey = pPublicRSAKey;
        }
    }
}
