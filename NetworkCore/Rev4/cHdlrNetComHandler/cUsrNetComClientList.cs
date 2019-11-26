using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EndevFramework.NetworkCore
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
        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 1a │ F I E L D S   ( P R I V A T E )                        ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝    

        #region ═╣ F I E L D S   ( P R I V A T E ) ╠═ 

        private readonly List<NetComCData> LClients = new List<NetComCData>();

        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 2b │ P R O P E R T I E S   ( P U B L I C )                  ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝  

        #region ═╣ P R O P E R T I E S   ( P U B L I C ) ╠═ 

        /// <summary>
        /// Gets the amount of clients in the list.
        /// </summary>
        public int Count
        {
            get => LClients.Count;
        }

        /// <summary>
        /// Gets the Clients data by its index.
        /// </summary>
        /// <param name="idx">Index of the user</param>
        /// <returns>ClientData-Object</returns>
        public NetComCData this[int idx]
        {
            get
            {
                if (LClients.Count > idx) return LClients[idx];
                else return null;
            }
        }

        /// <summary>
        /// Gets the Clients data by its username.
        /// </summary>
        /// <param name="pUsername">Username of the user</param>
        /// <returns>ClientData-Object</returns>
        public NetComCData this[string pUsername]
        {
            get
            {
                foreach (NetComCData client in LClients)
                    if (client.Username == pUsername) return client;
                return null;
            }
        }

        /// <summary>
        /// Gets the Clients data by its socket.
        /// </summary>
        /// <param name="pSocket">Socket of the user</param>
        /// <returns>ClientData-Object</returns>
        public NetComCData this[Socket pSocket]
        {
            get
            {
                foreach (NetComCData client in LClients)
                    if (client.LocalSocket == pSocket) return client;
                return null;
            }
        }

        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 4d │ M E T H O D S   ( P U B L I C )                        ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝ 

        #region ═╣ M E T H O D S   ( P U B L I C ) ╠═ 

        /// <summary>
        /// Adds a user.
        /// </summary>
        /// <param name="pSocket">Socket of the user</param>
        public void Add(Socket pSocket) => Add(pSocket, null, null);

        /// <summary>
        /// Adds a user.
        /// </summary>
        /// <param name="pSocket">Socket of the user</param>
        /// <param name="pUsername">Username of the user</param>
        public void Add(Socket pSocket, string pUsername) => Add(pSocket, pUsername, null);

        /// <summary>
        /// Adds a user.
        /// </summary>
        /// <param name="pSocket">Socket of the user</param>
        /// <param name="pUsername">Username of the user</param>
        /// <param name="pPassword">Password of the user</param>
        public void Add(Socket pSocket, string pUsername, string pPassword)
        {
            NetComCData cData = new NetComCData();
            cData.SetUserData(pUsername, pPassword);
            cData.SetUserSocket(pSocket);
            LClients.Add(cData);
        }

        /// <summary>
        /// Removes a user.
        /// </summary>
        /// <param name="pIndex">Index of the user</param>
        public void Remove(int pIndex)
        {
            LClients.RemoveAt(pIndex);
        }

        /// <summary>
        /// Removes a user.
        /// </summary>
        /// <param name="pUsername">Username of the user</param>
        public void Remove(string pUsername)
        {
            for (int i = 0; i < LClients.Count; i++)
                if (LClients[i].Username == pUsername) LClients.RemoveAt(i);
        }

        /// <summary>
        /// Removes a user.
        /// </summary>
        /// <param name="pSocket">Socket of the user</param>
        public void Remove(Socket pSocket)
        {
            for (int i = 0; i < LClients.Count; i++)
                if (LClients[i].LocalSocket == pSocket) LClients.RemoveAt(i);
        }

        /// <summary>
        /// Returns all clients from the list.
        /// Required for IEnumerable-Interface.
        /// </summary>
        /// <returns>NetComCData enumerator</returns>
        public IEnumerator GetEnumerator()
        {
            foreach (NetComCData client in LClients)
                yield return client;
        }

        #endregion
    }
}
