using EndevFrameworkNetworkCoreRev1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetComMixedSyncClient
{
    class NetComMSyncClient
    {
        static Socket ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static NetComClientList<NetComClientData> LClients = new NetComClientList<NetComClientData>();
        static byte[] Buffer = new byte[1048576];
        static void Main(string[] args)
        {
            // Connect to server
            while (!ClientSocket.Connected)
            {
                try { ClientSocket.Connect(IPAddress.Parse("127.0.0.1"), 2225); }
                catch (SocketException) { }
            }


            ClientSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(OnServerReceive), null);

            Console.ReadLine();
        }

        static void OnServerReceive(IAsyncResult AR)
        {
            int received = ClientSocket.EndReceive(AR);
            byte[] tmpBuffer = new byte[received];
            Array.Copy(Buffer, tmpBuffer, received);

            Console.WriteLine(Encoding.ASCII.GetString(tmpBuffer));

            ClientSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(OnServerReceive), null);
        }
    }
}
