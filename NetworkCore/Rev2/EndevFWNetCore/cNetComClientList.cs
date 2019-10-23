using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EndevFWNetCore
{
    public class NetComClientList : IEnumerable
    {
        private List<NetComClientData> LClients = new List<NetComClientData>();

        public int Count
        {
            get => LClients.Count;
        }

        public NetComClientData this[int idx]
        {
            get
            {
                if (LClients.Count > idx) return LClients[idx];
                else return null;
            }
        }

        public NetComClientData this[string pUsername]
        {
            get
            {
                foreach (NetComClientData client in LClients)
                    if (client.Username == pUsername) return client;
                return null;
            }
        }

        public NetComClientData this[Socket pSocket]
        {
            get
            {
                foreach (NetComClientData client in LClients)
                    if (client.Socket == pSocket) return client;
                return null;
            }
        }

        public void Add(Socket pSocket) => Add(pSocket, null, null);
        public void Add(Socket pSocket, string pUsername) => Add(pSocket, pUsername, null);

        public void Add(Socket pSocket, string pUsername, string pPassword)
        {
            LClients.Add(new NetComClientData(pSocket, pUsername, pPassword));
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
            foreach (NetComClientData client in LClients)
                yield return client;
        }
    }
}
