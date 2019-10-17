using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndevFrameworkNetworkCoreRev1
{
    class NetComException : Exception
    {
        public NetComException() { }
        public NetComException(string message) : base(message) { }
        public NetComException(string message, Exception innerException) : base(message, innerException) { }
    }
}
