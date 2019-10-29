using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndevFWNetCore
{
    public class NetComException : Exception
    {
        public NetComException() { }
        public NetComException(string message) : base(message) { }
        public NetComException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class NetComNotImplementedException : NotImplementedException
    {
        public NetComNotImplementedException() { }
        public NetComNotImplementedException(string message) : base(message) { }
        public NetComNotImplementedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
