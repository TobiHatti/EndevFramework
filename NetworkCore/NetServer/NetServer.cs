using EndevFrameworkNetworkCoreRev1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetServer
{
    class NetServer
    {
        static void Main(string[] args)
        {
            // Notes about Port 2225: For this port exist no active reservations from the IANA. 
            // Additionally no special permissions are required
            NetComServer server = new NetComServer(2225);

            server.Debug = NetComDebugOutput.ToConsole;
            server.EncodeMessage = NetComMessageEncoder.Default;
            server.ParseMessage = NetComMessageParser.Default;
            server.LibraryExec = NetComLibraryExecuter.Default;

            server.Init();
            server.Start();
            server.EnableProcessing();
            server.EnableSending();

            int i = 0;
            while(i < 100)
            {
                //server.SendToClient("[MyData:MyValue];[MyData2:MyValue2]", 0);
                server.Broadcast("[[UI i is brotkastl]]");

                Thread.Sleep(3000);
            }

            Console.ReadLine();
        }
    }
}
