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
    public class NetComClient : NetComUser
    {
        //===================================================================================================================
        //===================================================================================================================
        //=         PROPERTIES                                                                                              =
        //===================================================================================================================
        //===================================================================================================================

        #region -=[- FIELDS -]=-

        private const int BufferSize = 1048576; // 1 MB

        private int Port = 2225;
        private IPAddress ServerIP = null;
        private Socket ClientSocket = null;

        private Thread CommandProcessingThread = null;
        private Thread CommandSendingThread = null;
        private Thread CommandReceptionThread = null;

        private byte[] Buffer = new byte[BufferSize];

        #endregion

        #region -=[- PROPERTIES -]=-

        public string Username { get; private set; } = null;
        public string Password { get; private set; } = null;
        public int ThreadSleep { get; set; } = 0;
        public string ServerPublicKey { get; set; } = null;
        public NetComInstructionQueue IncommingInstructions { get; private set; } = new NetComInstructionQueue();
        public NetComInstructionQueue OutgoingInstructions { get; private set; } = new NetComInstructionQueue();
        #endregion

        #region -=[- DELEGATES -]=-

        // Debug output
        public delegate void DebugOutput(string pMessage, params object[] pParameters);
        public DebugOutput Debug { get; set; } = null;
        public object[] DebugParams { get; set; } = null;
        // Message Parser
        public delegate string MessageParser(string pMessage);
        public MessageParser ParseMessage { get; set; } = null;

        // Message Encoder
        public delegate string MessageEncoder(string pMessage);
        public MessageEncoder EncodeMessage { get; set; } = null;

        #endregion

        //===================================================================================================================
        //===================================================================================================================
        //=         CONSTRUCTORS                                                                                            =
        //===================================================================================================================
        //===================================================================================================================

        #region -=[- CONSTRUCTORS -]=-

        public NetComClient(string pServerIP, int pPort, string pUsername, string pPassword)
        {
            Port = pPort;
            ServerIP = IPAddress.Parse(pServerIP);

            Username = pUsername;
            Password = pPassword;

            NetComUser.LocalUser = this;
        }

        #endregion

        //===================================================================================================================
        //===================================================================================================================
        //=         CALLABLE METHODS                                                                                        =
        //===================================================================================================================
        //===================================================================================================================

        #region -=[- CALLABLE METHODS -]=-

        #region -=[- CLIENT STATE -]=-

        public void Start()
        {
            Debug("Setting up client...", DebugParams);
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Debug("Client setup complete!", DebugParams);

            TryConnect();

            CommandProcessingThread = new Thread(InstructionProcessingLoop);
            CommandProcessingThread.Start();

            CommandSendingThread = new Thread(InstructionSendingLoop);
            CommandSendingThread.Start();

            CommandReceptionThread = new Thread(InstructionReceptionLoop);
            CommandReceptionThread.Start();
        }

        #endregion

        private void InstructionProcessingLoop()
        {
            while (true)
            {
                ProcessNextInstruction();
                Thread.Sleep(ThreadSleep);
            }
        }

        private void ProcessNextInstruction()
        {
            if (IncommingInstructions.Count > 0)
            {
                IncommingInstructions[0].Instruction.Execute();
                IncommingInstructions.RemoveAt(0);
            }
            // TODO
            // Check if User is Authenticated
            // Else Reply to Client with the same message as sent, 
            // and the error that the user is not authenticated
        }

        private void InstructionSendingLoop()
        {
            while (true)
            {
                SendNextInstruction();
                Thread.Sleep(ThreadSleep);
            }
        }

        private void SendNextInstruction()
        {
            if (OutgoingInstructions.Count > 0)
            {
                byte[] buffer;

                string instruction;

                if (OutgoingInstructions[0].RSAEncrypted && ServerPublicKey != null)
                {
                    instruction = OutgoingInstructions[0].Instruction.Encode(true, ServerPublicKey);
                }
                else
                {
                    instruction = OutgoingInstructions[0].Instruction.Encode(false);
                }

                buffer = Encoding.UTF8.GetBytes(instruction);

                ClientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
                Debug($"Sent Message: {instruction}.", DebugParams);
                OutgoingInstructions.RemoveAt(0);
            }
        }

        private void InstructionReceptionLoop()
        {
            while (true)
            {
                ReceiveNextInstruction();
                Thread.Sleep(ThreadSleep);
            }
        }

        private void ReceiveNextInstruction()
        {
            Buffer = new byte[BufferSize];

            int received = ClientSocket.Receive(Buffer, SocketFlags.None);
            if (received == 0) return;

            byte[] data = new byte[received];
            Array.Copy(Buffer, data, received);
            string text = Encoding.UTF8.GetString(data);

            Console.ForegroundColor = ConsoleColor.Green;
            Debug("Received Message: " + text, DebugParams);
            Console.ForegroundColor = ConsoleColor.White;

            NetComInstruction[] instructionList = NetComInstruction.Parse(this, text).ToArray();

            foreach (NetComInstruction instr in instructionList)
                IncommingInstructions.Add(instr, null);
        }

        private void TryConnect()
        {
            int attempts = 0;
            while (!ClientSocket.Connected)
            {
                try
                {
                    attempts++;
                    Console.WriteLine("Connection attempt " + attempts);

                    ClientSocket.Connect(ServerIP, Port);
                }
                catch (SocketException)
                {
                    Console.Clear();
                }
            }
        }

        #endregion

        //-------------------------------------------------------------------------------------------------------------------
        //-         SENDING                                                                                                 -
        //-------------------------------------------------------------------------------------------------------------------

        public void Send(NetComInstruction pInstruction)
        {
            OutgoingInstructions.Add(pInstruction, null);
        }

        public void SendRSA(NetComInstruction pInstruction)
        {
            OutgoingInstructions.AddRSA(pInstruction, null);
        }
    }
}
