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
        public NetComClientData Client { get; private set; } = null;
        public bool RSAEncrypted { get; private set; } = false;

        public NetComInstructionQueueElement(string pInstruction, NetComClientData pClient) : this(pInstruction, pClient, false) { }

        public NetComInstructionQueueElement(string pInstruction, NetComClientData pClient, bool pRSAEncrypted)
        {
            Instruction = pInstruction;
            Client = pClient;
            RSAEncrypted = pRSAEncrypted;
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
            get
            {
                if (LInstructions.Count > idx) return LInstructions[idx];
                else return null;
            }
        }

        public NetComInstructionQueueElement this[Socket pSocket]
        {
            get
            {
                foreach (NetComInstructionQueueElement instruction in LInstructions)
                    if (instruction.Client.Socket == pSocket) return instruction;
                return null;
            }
        }

        public void Add(string pInstruction, NetComClientData pClient)
        {
            LInstructions.Add(new NetComInstructionQueueElement(pInstruction, pClient));
        }

        public void AddRSA(string pInstruction, NetComClientData pClient)
        {
            LInstructions.Add(new NetComInstructionQueueElement(pInstruction, pClient, true));
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
