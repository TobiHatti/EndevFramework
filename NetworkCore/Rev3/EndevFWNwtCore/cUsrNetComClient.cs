using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
    /// Acting as a client.
    /// </summary>
    public class NetComClient : NetComOperator
    {
        private Thread instructionReceptionThread = null;

        public NetComClient(string pServerIP, int pPort)
        {
            port = pPort;
            serverIP = IPAddress.Parse(pServerIP);
        }

        public override void AsyncInstructionProcessNext()
        {
            throw new NotImplementedException();
        }

        public override void AsyncInstructionSendNext()
        {
            throw new NotImplementedException();
        }
    }
}
