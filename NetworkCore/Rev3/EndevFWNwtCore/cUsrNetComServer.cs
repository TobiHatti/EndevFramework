using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Main Handler for Net-Communication.     <para />     
    /// Acting as a server.
    /// </summary>
    public class NetComServer : NetComOperator
    {
        private ClientList LClients = new ClientList();

        public NetComServer(int pPort)
        {
            port = pPort;
        }
    }
}
