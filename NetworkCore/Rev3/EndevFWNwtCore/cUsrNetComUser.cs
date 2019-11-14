using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
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
    /// Basic object for NetCom-Users.
    /// </summary>
    public class NetComUser
    {
        internal Socket LocalSocket { get; set; } = null;
        internal string Username { get; set; } = null;
        internal string Password { get; set; } = null;
        internal RSAKeyPair RSAKeys { get; set; }

        /// <summary>
        /// Sets the LocalSocket of the user.
        /// </summary>
        /// <param name="pSocket">Users socket</param>
        internal void SetUserSocket(Socket pSocket)
        {
            LocalSocket = pSocket;
        }

        /// <summary>
        /// Sets the users user-data.
        /// </summary>
        /// <param name="pUsername">Users username</param>
        /// <param name="pPassword">Users password</param>
        /// <param name="pPublicKey">Users public-key</param>
        internal void SetUserData(string pUsername, string pPassword, string pPublicKey = null)
        {
            Username = pUsername;
            Password = pPassword;

            if(pPublicKey != null)
            {
                RSAKeyPair keys = new RSAKeyPair
                {
                    PublicKey = pPublicKey,
                    PrivateKey = null
                };
                RSAKeys = keys;
            }
        }

        /// <summary>
        /// Returns the username of the user.
        /// </summary>
        /// <returns>Users username</returns>
        public override string ToString()
        {
            return Username;
        }
    }
}
