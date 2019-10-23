using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EndevFWNetCore
{
    public class NetComClientListElement
    {
        public Socket Socket { get; private set; } = null;
        public string Username { get; private set; } = null;
        public bool Authenticated { get; private set; } = false;
        public string PublicKey { get; private set; } = null;

        public NetComClientListElement(Socket pSocket, string pUsername) : this(pSocket, pUsername, null) { }

        public NetComClientListElement(Socket pSocket, string pUsername, string pPassword)
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
    }

    public class NetComClientList : IEnumerable
    {
        private List<NetComClientListElement> LClients = new List<NetComClientListElement>();

        public int Count
        {
            get => LClients.Count;
        }

        public NetComClientListElement this[int idx]
        {
            get
            {
                if (LClients.Count > idx) return LClients[idx];
                else return null;
            }
        }

        public NetComClientListElement this[string pUsername]
        {
            get
            {
                foreach (NetComClientListElement client in LClients)
                    if (client.Username == pUsername) return client;
                return null;
            }
        }

        public NetComClientListElement this[Socket pSocket]
        {
            get
            {
                foreach (NetComClientListElement client in LClients)
                    if (client.Socket == pSocket) return client;
                return null;
            }
        }

        public void Add(Socket pSocket) => Add(pSocket, null, null);
        public void Add(Socket pSocket, string pUsername) => Add(pSocket, pUsername, null);

        public void Add(Socket pSocket, string pUsername, string pPassword)
        {
            LClients.Add(new NetComClientListElement(pSocket, pUsername, pPassword));
        }

        public void RemoveAt(int pIndex)
        {
            LClients.RemoveAt(pIndex);
        }

        public void RemoveAt(string pUsername)
        {
            for (int i = 0; i < LClients.Count; i++)
                if (LClients[i].Username == pUsername) LClients.RemoveAt(i);
        }

        public void RemoveAt(Socket pSocket)
        {
            for (int i = 0; i < LClients.Count; i++)
                if (LClients[i].Socket == pSocket) LClients.RemoveAt(i);
        }

        public IEnumerator GetEnumerator()
        {
            foreach (NetComClientListElement client in LClients)
                yield return client;
        }
    }
}
