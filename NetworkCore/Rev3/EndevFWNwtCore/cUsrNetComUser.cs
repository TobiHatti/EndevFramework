using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
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
    /// Basic object for NetCom-Users
    /// </summary>
    public class NetComUser
    {
        public Socket LocalSocket { get; protected set; } = null;
        

        public string Username { get; protected set; } = null;
        public string Password { get; protected set; } = null;
        public RSAKeyPair RSAKeys { get; protected set; }

        public void SetUserSocket(Socket pSocket)
        {
            LocalSocket = pSocket;
        }

        public void SetUserData(string pUsername, string pPassword, string pPublicKey = null)
        {
            Username = pUsername;
            Password = pPassword;

            if(pPublicKey != null)
            {
                RSAKeyPair keys = new RSAKeyPair();
                keys.PublicKey = null;
                keys.PrivateKey = pPublicKey;
                RSAKeys = keys;
            }
        }
    }
}
