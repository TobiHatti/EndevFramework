using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

// DEPRECATED (05.11.2019) - Not more functionality than a generic list

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
        private List<InstructionBase> LInstructions = new List<InstructionBase>();

        public int Count
        {
            get => LInstructions.Count;
        }

        public InstructionBase this[int idx]
        {
            get
            {
                if (LInstructions.Count > idx) return LInstructions[idx];
                else return null;
            }
        }

        public void Add(InstructionBase pInstruction)
        {
            LInstructions.Add(pInstruction);
        }

        public IEnumerator GetEnumerator()
        {
            throw new NetComNotImplementedException();
        }
    }
}
