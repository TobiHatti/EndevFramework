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
    /// Basic object for NetCom-Users
    /// </summary>
    public class NetComUser
    {
        internal Socket LocalSocket { get; set; } = null;
        internal string Username { get; set; } = null;
        internal string Password { get; set; } = null;
        internal RSAKeyPair RSAKeys { get; set; }

        internal void SetUserSocket(Socket pSocket)
        {
            LocalSocket = pSocket;
        }

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

        public override string ToString()
        {
            return Username;
        }
    }
}
