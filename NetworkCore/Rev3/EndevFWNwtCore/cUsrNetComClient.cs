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

        public NetComClient(string pServerIP, int pPort)
        {
            port = pPort;
            serverIP = IPAddress.Parse(pServerIP);
        }


        public void Start()
        {
            Debug("Setting up client...");
            LocalSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Debug("Client setup complete!");

            TryConnect();

            instructionProcessingThread = new Thread(AsyncInstructionProcessNext);
            instructionProcessingThread.Start();

            instructionSendingThread = new Thread(AsyncInstructionSendNext);
            instructionSendingThread.Start();

            instructionReceptionThread = new Thread(AsyncInstructionReceiveNext);
            instructionReceptionThread.Start();
        }

        private void TryConnect()
        {
            int attempts = 0;
            while (!LocalSocket.Connected)
            {
                try
                {
                    attempts++;
                    Console.WriteLine("Connection attempt " + attempts);

                    LocalSocket.Connect(serverIP, port);
                }
                catch (SocketException)
                {

                }
            }
        }

        public void Send(InstructionBase pInstruction)
        {
            outgoingInstructions.Add(pInstruction);
        }

        protected void AsyncInstructionReceptionLoop()
        {
            while (true)
            {
                AsyncInstructionReceiveNext();
            }
        }


        protected override void AsyncInstructionProcessNext()
        {

        }

        protected override void AsyncInstructionSendNext()
        {
            byte[] buffer;

            buffer = Encoding.UTF8.GetBytes(outgoingInstructions[0].Encode());

            LocalSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
            Debug($"Sent Message: {instruction}.", DebugParams);
            OutgoingInstructions.RemoveAt(0);
        }
        
        protected void AsyncInstructionReceiveNext()
        {

        }
    }
}
