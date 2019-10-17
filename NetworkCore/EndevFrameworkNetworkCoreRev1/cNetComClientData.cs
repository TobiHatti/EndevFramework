using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EndevFrameworkNetworkCoreRev1
{
    public class NetComClientData
    {
        public Socket Socket { get; private set; } = null;
        public string Username { get; private set; } = null;
        public int Hash { get => Socket.GetHashCode(); }
        public bool Authenticated { get; private set; } = false;

        public NetComClientData(Socket pSocket)
        {
            this.Socket = pSocket;
        }

        public bool Authenticate(string pUsername, string pPassword)
        {
            // Check if credentials are valid

            Username = pUsername;
            Authenticated = true;

            return true;
        }
    }
}
