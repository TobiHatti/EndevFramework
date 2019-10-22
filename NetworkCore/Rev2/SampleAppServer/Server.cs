using EndevFWNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SampleAppServer
{
    class Server
    {
        static void Main(string[] args)
        {

            NetComServer server = new NetComServer(2225);

            server.Debug = NetComDebugOutput.ToConsole;

            server.Start(); // Listening starts with server.Start();

            while(true)
            {
                server.SendTest();
                Thread.Sleep(200);
            }

        }
    }
}
