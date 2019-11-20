using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndevFrameworkNetworkCore
{
    public class InstructionOutLog
    {
        private List<InstructionBase> instructions = new List<InstructionBase>();
        private List<int> resendAttempts = new List<int>();

        public int Count
        {
            get => instructions.Count;
        }

        public InstructionBase this[int pIndex]
        {
            get
            {
                if (instructions.Count > pIndex) return instructions[pIndex];
                else return null;
            }
        }

        public void Add(InstructionBase pInstruction)
        {
            instructions.Add(pInstruction);
            resendAttempts.Add(0);
        }

        public void RemoveAt(int pIndex)
        {
            instructions.RemoveAt(pIndex);
            resendAttempts.RemoveAt(pIndex);
        }

        public int GetAttempts(int pIndex)
        {
            return resendAttempts[pIndex];
        }

        public void AddAttempt(int pIndex)
        {
            resendAttempts[pIndex]++;
        }
    }
}
