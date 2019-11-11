using EndevFWNwtCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ILE = EndevFWNwtCore.InstructionLibraryExtension;
namespace SampleAppServer
{
    class Server
    {
        static void Main(string[] args)
        {
            Console.Title = "Server";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("===================================");
            Console.WriteLine("=            S E R V E R          =");
            Console.WriteLine("===================================\r\n");



            NetComServer server = new NetComServer(2225);

            server.SetDebugOutput(DebugOutput.ToConsole);

            NetComCData.SetAuthenticationTool(AuthenticationTools.DebugAuth);

            server.Start();

            // The server can send to a range of connected clients, wich can be selected in the server.ConnectedClients-Property

            Thread.Sleep(2000);

            while(true)
            {
                server.Send(new ILE.MyStabilityTest(server, server.ConnectedClients[new Random().Next(0, server.ConnectedClients.Count)]));
                
                Thread.Sleep(1000);

                server.Broadcast(new ILE.MyStabilityTest(server, null));

                Thread.Sleep(1000);
            }
            

            Console.ReadKey();

        }
    }
}
