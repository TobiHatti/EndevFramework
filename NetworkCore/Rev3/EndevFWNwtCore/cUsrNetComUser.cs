using System;
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
    /// Basic object for NetCom-Users
    /// </summary>
    public class NetComUser
    {
        protected Socket localSocket = null;
        protected bool authenticated = false;

        public string Username { get; private set; } = null;
        public string Password { get; private set; } = null;
        public RSAKeyPair RSAKeys { get; private set; } 
    }
}
