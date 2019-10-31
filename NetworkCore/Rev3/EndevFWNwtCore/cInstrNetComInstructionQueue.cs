using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndevFWNwtCore
{
    /// <summary>
    /// =====================================   <para />
    /// FRAMEWORK: EndevFrameworkNetworkCore    <para />
    /// SUB-PACKAGE: Instruction-Objects        <para />
    /// =====================================   <para />
    /// DESCRIPTION:                            <para />
    /// Stores instruction and 
    /// assigns them to a socket
    /// </summary>
    public class InstructionQueue : IEnumerable
    {


        public IEnumerator GetEnumerator()
        {
            throw new NetComNotImplementedException();
        }
    }
}
