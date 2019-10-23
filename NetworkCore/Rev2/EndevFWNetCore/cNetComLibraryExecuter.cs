using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndevFWNetCore
{
    public class NetComLibraryExecuter
    {
        public static object[] DefaultServer(string pMessage, NetComClientListElement pClient)
        {
            return new object[] { pMessage };
        }

        public static object[] DefaultClient(string pMessage)
        {
            return new object[] { pMessage };
        }
    }
}
