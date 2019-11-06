﻿using System;
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
                catch (SocketException)
                {

                }
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
            Debug($"Sent Message: {outgoingInstructions[0].ToString()}");
            Debug($"Sent Message: {outgoingInstructions[0].Encode()}");

            outgoingInstructions.RemoveAt(0);
        }
        
        protected void AsyncInstructionReceiveNext()
        {

        }
    }
}
