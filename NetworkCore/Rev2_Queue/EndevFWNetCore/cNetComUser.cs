using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndevFWNetCore
{
    public class NetComUser
    {
        public static NetComUser LocalUser { get; set; } = null;

        public NetComRSAHandler RSA { get; private set; } = new NetComRSAHandler();
    }
}
