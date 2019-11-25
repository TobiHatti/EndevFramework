using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EndevFrameworkNetworkCore
{
    public class NetComClientHandler
    {
        private NetComClient client = null;
        private int port = 0;
        private IPAddress serverIP = null;
        private RSAKeyPair rsaKeys;

        public NetComClient Client { get => client; }

        public NetComClientHandler(string pServerIP, int pPort)
            : this(IPAddress.Parse(pServerIP), pPort) { }

        public NetComClientHandler(IPAddress pServerIP, int pPort)
        {
            rsaKeys = RSAHandler.GenerateKeyPair();
            port = pPort;
            serverIP = pServerIP;
        }

        public void Start()
        {
            try
            {
                client = new NetComClient(serverIP, port, rsaKeys);
                client.Start();
            }
            catch
            {
                Start();
            }
        }




    }
}
