using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EndevFrameworkNetworkCore
{
    /// <summary>
    /// =====================================   <para />
    /// FRAMEWORK: EndevFrameworkNetworkCore    <para />
    /// SUB-PACKAGE: User-Objects               <para />
    /// =====================================   <para />
    /// DESCRIPTION:                            <para />
    /// Stores multiple connected clients 
    /// connected to the server
    /// </summary>
    public class ClientList : IEnumerable
    {
        private readonly List<NetComCData> LClients = new List<NetComCData>();

        public int Count
        {
            get => LClients.Count;
        }

        public NetComCData this[int idx]
        {
            get 
            {
                if (LClients.Count > idx) return LClients[idx];
                else return null;
            }
        }

        public NetComCData this[string pUsername]
        {
            get
            {
                foreach (NetComCData client in LClients)
                    if (client.Username == pUsername) return client;
                return null;
            }
        }

        public NetComCData this[Socket pSocket]
        {
            get
            {
                foreach (NetComCData client in LClients)
                    if (client.LocalSocket == pSocket) return client;
                return null;
            }
        }

        public void Add(Socket pSocket) => Add(pSocket, null, null);
        public void Add(Socket pSocket, string pUsername) => Add(pSocket, pUsername, null);
        public void Add(Socket pSocket, string pUsername, string pPassword)
        {
            NetComCData cData = new NetComCData();
            cData.SetUserData(pUsername, pPassword);
            cData.SetUserSocket(pSocket);
            LClients.Add(cData);
        }


        public void Remove(int pIndex)
        {
            LClients.RemoveAt(pIndex);
        }

        public void Remove(string pUsername)
        {
            for (int i = 0; i < LClients.Count; i++)
                if (LClients[i].Username == pUsername) LClients.RemoveAt(i);
        }

        public void Remove(Socket pSocket)
        {
            for (int i = 0; i < LClients.Count; i++)
                if (LClients[i].LocalSocket == pSocket) LClients.RemoveAt(i);
        }

        public IEnumerator GetEnumerator()
        {
            foreach (NetComCData client in LClients)
                yield return client;
        }
    }
}
