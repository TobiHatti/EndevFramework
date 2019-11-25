using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndevFrameworkNetworkCore
{
    public class NetComServerHandler
    {
        private NetComServer server = null;
        private int port = 0;
        private RSAKeyPair rsaKeys;

        public NetComServer Server { get => server; }

        public NetComServerHandler(int pPort)
        {
            rsaKeys = RSAHandler.GenerateKeyPair();
            port = pPort;
        }

        public void Start()
        {
            try
            {
                server = new NetComServer(port, rsaKeys);
                server.Start();
            }
            catch
            {
                Start();
            }
        }
    }
}
