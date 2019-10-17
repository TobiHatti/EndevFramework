using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EndevFrameworkNetworkCoreRev1
{
    public class NetComInstructionQueue<TInstruction, TSocket> where TSocket : Socket
    {
        public List<TInstruction> LInstructions { get; set; } = new List<TInstruction>();
        public List<TSocket> LSocket { get; set; } = new List<TSocket>();

        public int Count
        {
            get => LInstructions.Count;
        }

        public void Add(TInstruction pInstruction, TSocket pSocket)
        {
            LInstructions.Add(pInstruction);
            LSocket.Add(pSocket);
        }

        public KeyValuePair<TInstruction,TSocket> this[int idx]
        {
            get => new KeyValuePair<TInstruction, TSocket>(LInstructions[idx], LSocket[idx]);
        }

        public void RemoveAt(int pIndex)
        {
            try
            {
                LInstructions.RemoveAt(pIndex);
                LSocket.RemoveAt(pIndex);
            }
            catch
            {

            }
        }
    }
}
