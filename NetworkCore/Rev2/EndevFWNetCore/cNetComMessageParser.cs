using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndevFWNetCore
{
    public class NetComMessageParser
    {
        public static string DefaultServer(string pMessage, NetComClientListElement pClient)
        {
            return pMessage;
        }

        public static string DefaultClient(string pMessage)
        {
            return pMessage;
        }
    }
}
