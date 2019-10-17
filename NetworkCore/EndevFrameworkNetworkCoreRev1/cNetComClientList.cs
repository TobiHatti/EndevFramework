using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EndevFrameworkNetworkCoreRev1
{
    public class NetComClientList<T> where T : NetComClientData
    {
        public List<T> ClientList { get; private set; } = new List<T>();
        public int Count { get => ClientList.Count; }

        public NetComClientData this[int idx]
        {
            get
            {
                if (idx < Count) return ClientList[idx];
                else return null;
            }
        }

        public NetComClientData this[long hash]
        {
            get
            {
                foreach (T client in ClientList) 
                    if (client.Hash == hash) return client;
                return null;
            }
        }

        public NetComClientData this[string username]
        {
            get
            {
                foreach (T client in ClientList)
                    if (client.Username == username) return client;
                return null;
            }
        }

        public NetComClientData this[Socket socket]
        {
            get
            {
                foreach (T client in ClientList)
                    if (client.Socket == socket) return client;
                return null;
            }
        }

        public void Add(T pData)
        {
            ClientList.Add(pData);
        }

        public bool Authenticated(Socket pSocket)
        {
            return true;

            for (int i = 0; i < Count; i++)
                if (ClientList[i].Socket == pSocket && ClientList[i].Authenticated) return true;
            return false;
        }
    }
}
