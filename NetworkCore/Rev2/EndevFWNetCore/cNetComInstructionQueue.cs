using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EndevFWNetCore
{
    public class NetComInstructionQueueElement
    {
        public string Instruction { get; private set; } = null;
        public Socket Socket { get; private set; } = null;

        public NetComInstructionQueueElement(string pInstruction, Socket pSocket)
        {
            Instruction = pInstruction;
            Socket = pSocket;
        }
    }

    public class NetComInstructionQueue
    {
        private List<NetComInstructionQueueElement> LInstructions = new List<NetComInstructionQueueElement>();

        public int Count
        {
            get => LInstructions.Count;
        }

        public NetComInstructionQueueElement this[int idx]
        {
            get => LInstructions[idx];
        }

        public NetComInstructionQueueElement this[Socket pSocket]
        {
            get
            {
                foreach (NetComInstructionQueueElement instruction in LInstructions)
                    if (instruction.Socket == pSocket) return instruction;
                return null;
            }
        }

        public void Add(string pInstruction, Socket pSocket)
        {
            LInstructions.Add(new NetComInstructionQueueElement(pInstruction, pSocket));
        }

        public void RemoveAt(int pIndex)
        {
            if(LInstructions.Count > pIndex)
            {
                LInstructions.RemoveAt(pIndex);
            }
        }

    }
}
