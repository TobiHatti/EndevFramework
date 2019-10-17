using EndevFrameworkNetworkCoreRev1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetComMixedSyncServer
{
    class NetComMSyncServer
    {
        static Socket ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static NetComClientList<NetComClientData> LClients = new NetComClientList<NetComClientData>();
        static byte[] Buffer = new byte[1048576];  
        static byte[] Data = new byte[1048576];  


        static void Main(string[] args)
        {
            // Server Setup
            ServerSocket.Bind(new IPEndPoint(IPAddress.Any, 2225));
            ServerSocket.Listen(20);

            ServerSocket.BeginAccept(new AsyncCallback(OnClientAccept), null);


            while(true)
            {
                if (LClients.Count > 0)
                    ServerSocket.Send(Data, 0, Data.Length, SocketFlags.Broadcast);
            }

            Console.ReadLine();
        }

        static void OnClientAccept(IAsyncResult AR)
        {
            // Accept the client socket and add it to the socket-list
            Socket clientSocket = ServerSocket.EndAccept(AR);
            LClients.Add(new NetComClientData(clientSocket));
            clientSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(OnClientReceive), clientSocket);

            // Re-Open the server-socket for further connections
            ServerSocket.BeginAccept(new AsyncCallback(OnClientAccept), null);
        }

        static void OnClientReceive(IAsyncResult AR)
        {
            Socket clientSocket = (Socket)AR.AsyncState;
            int received = clientSocket.EndReceive(AR);
            byte[] tmpBuffer = new byte[received];
            Array.Copy(Buffer, tmpBuffer, received);

            Console.WriteLine(Encoding.ASCII.GetString(tmpBuffer));

            // Re-Open the server-socket for further connections
            ServerSocket.BeginAccept(new AsyncCallback(OnClientAccept), null);
        }
    }
}
