using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EndevFWNetCore
{
    public class NetComInstructionQueue
    {
        public List<string> LInstructions { get; set; } = new List<string>();
        public List<Socket> LSocket { get; set; } = new List<Socket>();

        public int Count
        {
            get => LInstructions.Count;
        }

        public void Add(string pInstruction, Socket pSocket)
        {
            LInstructions.Add(pInstruction);
            LSocket.Add(pSocket);
        }

        public KeyValuePair<string, Socket> this[int idx]
        {
            get
            {
                if (LInstructions.Count > idx && LSocket.Count > idx) return new KeyValuePair<string, Socket>(LInstructions[idx], LSocket[idx]);
                else return new KeyValuePair<string, Socket>();
            }
        }
        public void RemoveAt(int pIndex)
        {
            if(LInstructions.Count > pIndex && LSocket.Count > pIndex)
            {
                LInstructions.RemoveAt(pIndex);
                LSocket.RemoveAt(pIndex);
            }
        }

    }
}
