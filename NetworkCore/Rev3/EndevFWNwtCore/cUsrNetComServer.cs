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
        
        private ClientList LClients = new ClientList();

        public NetComServer(int pPort)
        {
            port = pPort;
            serverIP = IPAddress.Any;
        }

        protected override void AsyncInstructionSendNext()
        {
            throw new NotImplementedException();
        }

        protected override void AsyncInstructionProcessNext()
        {
            incommingInstructions[0].Execute();
            incommingInstructions.RemoveAt(0);
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
            foreach (NetComCData client in LClients)
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

            LClients.Add(socket);

            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, socket);
            Debug("New client connected!");

            //SendToClient(socket, new NCILib.PreAuth(this));

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
                Debug("Client forcefully disconnected.");
                // Don't shutdown because the socket may be disposed and its disconnected anyway.
                current.Close();
                LClients.Remove(current);
                return;
            }

            byte[] recBuf = new byte[received];
            Array.Copy(buffer, recBuf, received);
            string text = Encoding.UTF8.GetString(recBuf);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Debug("Received message: " + text);
            Console.ForegroundColor = ConsoleColor.White;


            InstructionBase[] instructionList = InstructionOperations.Parse(this, current, text, LClients).ToArray();

            foreach (InstructionBase instr in instructionList)
                incommingInstructions.Add(instr);

            

            current.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, current);
        }

    }
}
