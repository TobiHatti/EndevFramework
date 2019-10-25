using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EndevFWNetCore
{
    public class NetComClientData
    {
        public Socket Socket { get; private set; } = null;
        public string Username { get; private set; } = null;
        public bool Authenticated { get; private set; } = false;
        public string PublicKey { get; private set; } = null;

        public NetComClientData(Socket pSocket, string pUsername) : this(pSocket, pUsername, null) { }

        public NetComClientData(Socket pSocket, string pUsername, string pPassword)
        {
            Socket = pSocket;
            Username = pUsername;

            if (!string.IsNullOrEmpty(pPassword)) Authenticated = Authenticate(pPassword);
            else Authenticated = false;
        }

        public bool Authenticate(string pPassword)
        {
            if (true) return true;
            else return false;
        }

        public void SetPublicKey(string pPublicKey) => PublicKey = pPublicKey;

        public void SetUsername(string pUsername) => Username = pUsername;
    }
}
