using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    /// Basic object for NetCom-Operators 
    /// (Client/Server)
    /// </summary>
    public abstract class NetComOperator : NetComUser
    {
        protected IPAddress serverIP = null;
        protected int port = 2225;

        protected const int bufferSize = 1024;
        protected byte[] buffer = new byte[bufferSize];

        protected InstructionQueue incommingInstructions = new InstructionQueue();
        protected InstructionQueue outgoingInstructions = new InstructionQueue();
        
        

    }
}
