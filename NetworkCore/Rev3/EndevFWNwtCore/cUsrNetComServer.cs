using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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

        public override void AsyncInstructionSendNext()
        {
            throw new NotImplementedException();
        }

        public override void AsyncInstructionProcessNext()
        {
            throw new NotImplementedException();
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
            Debug("New client connected!", debugParams);

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
                Debug("Client forcefully disconnected.", debugParams);
                // Don't shutdown because the socket may be disposed and its disconnected anyway.
                current.Close();
                LClients.Remove(current);
                return;
            }

            byte[] recBuf = new byte[received];
            Array.Copy(buffer, recBuf, received);
            string text = Encoding.UTF8.GetString(recBuf);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Debug("Received message: " + text, debugParams);
            Console.ForegroundColor = ConsoleColor.White;


            InstructionBase[] instructionList = InstructionOperations.Parse(this, current, text, LClients).ToArray();

            foreach (InstructionBase instr in instructionList)
                incommingInstructions.Add(instr);

            current.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, current);
        }

    }
}
