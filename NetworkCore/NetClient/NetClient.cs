using EndevFrameworkNetworkCoreRev1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetClient
{
    class NetClient
    {
        static void Main(string[] args)
        {
            NetComClient client = new NetComClient("127.0.0.1", 2225);

            client.Debug = NetComDebugOutput.ToConsole;
            client.EncodeMessage = NetComMessageEncoder.Default;
            client.ParseMessage = NetComMessageParser.Default;
            client.LibraryExec = NetComLibraryExecuter.Default;

            client.Init();
            client.Start();
            client.EnableListening();
            client.EnableProcessing();
            //client.EnableSending();

            Console.ReadLine();
        }
    }
}
