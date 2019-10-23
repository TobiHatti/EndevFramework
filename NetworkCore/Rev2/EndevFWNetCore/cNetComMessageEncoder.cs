using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndevFWNetCore
{
    public class NetComMessageEncoder
    {
        public static string DefaultServer(string pMessage, NetComClientData pClient)
        {
            return pMessage;
        }

        public static string DefaultClient(string pMessage)
        {
            return pMessage;
        }
    }
}
