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
    /// Acting as a server.
    /// </summary>
    public class NetComServer : NetComOperator
    {
        public ClientList ConnectedClients { get; } = new ClientList();
        public NetComUser CurrentProcessingClient 
        { 
            get
            {
                if (incommingInstructions.Count > 0) return incommingInstructions[0].Sender;
                else return null;
            }
        }

        public NetComServer(int pPort)
        {
            port = pPort;
            serverIP = IPAddress.Any;

            RSAKeys = RSAHandler.GenerateKeyPair();
        }

  

        protected override void AsyncInstructionSendNext()
        {
            Socket current = outgoingInstructions[0]?.Receiver.LocalSocket;
            byte[] data;

            data = Encoding.UTF8.GetBytes(outgoingInstructions[0].Encode());

            current.Send(data);

            Debug($"Sent Message to {outgoingInstructions[0].Receiver.ToString()}.");
            Debug(outgoingInstructions[0].ToString());

            outgoingInstructions.RemoveAt(0);
        }

        /// <summary>
        /// Initializes and Starts the Server
        /// </summary>
        public void Start()
        {
            Debug("Setting up server...");

            LocalSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            LocalSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            LocalSocket.Listen(0);
            LocalSocket.BeginAccept(AcceptCallback, null);

            Debug("Server setup complete!");


            instructionProcessingThread = new Thread(AsyncInstructionProcessingLoop);
            instructionProcessingThread.Start();

            instructionSendingThread = new Thread(AsyncInstructionSendingLoop);
            instructionSendingThread.Start();
        }

        /// <summary>
        /// Properly closes all connections 
        /// and shuts down the server
        /// </summary>
        public void Shutdown()
        {
            Debug("Shutting down all connections...");
            foreach (NetComCData client in ConnectedClients)
            {
                client.LocalSocket.Shutdown(SocketShutdown.Both);
                client.LocalSocket.Close();
            }

            Debug("Shutting down server...");
            LocalSocket.Close();
            Debug("Shutdown complete!");
        }

        /// <summary>
        /// Gets called when a connection is established
        /// </summary>
        /// <param name="AR">IAsyncResult</param>
        private void AcceptCallback(IAsyncResult AR)
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
            Send(new InstructionLibraryEssentials.__AuthenticationServer2Client(this, ConnectedClients[socket]));

            LocalSocket.BeginAccept(AcceptCallback, null);
        }

        /// <summary>
        /// Gets called when a message is received
        /// </summary>
        /// <param name="AR">IAsyncResult</param>
        private void ReceiveCallback(IAsyncResult AR)
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
                foreach (InstructionBase instr in instructionList)
                    incommingInstructions.Add(instr);
            }
            catch(NetComAuthenticationException authEx)
            {
                Debug("Authentication-Error.");
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
                ConnectedClients.Remove(current);
                return;
            }

        }

        public void Send(InstructionBase pInstruction)
        {
            if (pInstruction.Receiver != null)
            {
                Debug($"Queueing message for {pInstruction.Receiver.ToString()}.");
                outgoingInstructions.Add(pInstruction);
            }
        }

        public void Broadcast(InstructionBase pInstruction)
        {
            InstructionBase tmpInstruction = null;
            foreach(NetComUser user in ConnectedClients)
            {
                tmpInstruction = pInstruction.Clone();
                tmpInstruction.Receiver = user;

                Debug($"Queueing message for {tmpInstruction.Receiver.ToString()}.");
                outgoingInstructions.Add(tmpInstruction);
            }
        }

        // SendGroup() - An benutzergruppen senden
        // SendList() - An mehrere clients senden


    }
}
