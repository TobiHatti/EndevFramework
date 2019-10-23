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
    public class NetComClient
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

        private string Username = null;
        private string Password = null;
        private string ServerPublicKey = null;

        private Thread CommandProcessingThread = null;
        private Thread CommandSendingThread = null;
        private Thread CommandReceptionThread = null;

        private byte[] Buffer = new byte[BufferSize];

        #endregion

        #region -=[- PROPERTIES -]=-

        public int ThreadSleep { get; set; } = 0;
        public NetComInstructionQueue IncommingInstructions { get; private set; } = new NetComInstructionQueue();
        public NetComInstructionQueue OutgoingInstructions { get; private set; } = new NetComInstructionQueue();

        public NetComRSAHandler RSA { get; private set; } = new NetComRSAHandler();

        #endregion

        #region -=[- DELEGATES -]=-

        // Debug output
        public delegate void DebugOutput(string pMessage, params object[] pParameters);
        public DebugOutput Debug { get; set; } = null;
        public object[] DebugParams { get; set; } = null;
        // Message Parser
        public delegate string MessageParser(string pMessage, params object[] pParameters);
        public MessageParser ParseMessage { get; set; } = null;

        // Message Encoder
        public delegate string MessageEncoder(string pMessage, params object[] pParameters);
        public MessageEncoder EncodeMessage { get; set; } = null;

        // Message Library
        public delegate object[] MessageLibraryExec(string pMessageKey, params object[] pParameters);
        public MessageLibraryExec LibraryExec { get; set; } = null;

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

                    string instruction = OutgoingInstructions[0].Instruction;
                    instruction = EncodeMessage(instruction, OutgoingInstructions[0].Client);

                    if (OutgoingInstructions[0].RSAEncrypted && ServerPublicKey != null)
                    {
                        buffer = Encoding.UTF8.GetBytes(RSA.Encrypt(instruction, OutgoingInstructions[0].Client.PublicKey));
                    }
                    else
                    {
                        buffer = Encoding.UTF8.GetBytes(instruction);
                    }

                    ClientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
                    Debug($"Sent Message: {OutgoingInstructions[0].Instruction}.", DebugParams);
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

            Debug("Received Message: " + text);
            IncommingInstructions.Add(text, null);
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

        public void Send(string pMessage)
        {
            OutgoingInstructions.Add(pMessage, null);
        }

        public void SendRSA(string pMessage)
        {
            OutgoingInstructions.AddRSA(pMessage, null);
        }
    }
}
