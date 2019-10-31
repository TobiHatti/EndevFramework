using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    /// Stores multiple connected clients 
    /// connected to the server
    /// </summary>
    public class ClientList : IEnumerable
    {
        private List<NetComCData> LClients = new List<NetComCData>();

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

        public IEnumerator GetEnumerator()
        {
            throw new NetComNotImplementedException();
        }
    }
}
