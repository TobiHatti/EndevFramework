using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndevFrameworkNetworkCoreRev1
{
    public class NetComMessageEncoder
    {
        public static string Default(string pMessage, params object[] pParameters)
        {
            return "{{" + pMessage + "}}";
        }
    }
}
