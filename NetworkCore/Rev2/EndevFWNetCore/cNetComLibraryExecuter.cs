using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndevFWNetCore
{
    public class NetComLibraryExecuter
    {
        public static object[] Default(string pMessage, params object[] pParameters)
        {
            return new object[] { pMessage };
        }
    }
}
