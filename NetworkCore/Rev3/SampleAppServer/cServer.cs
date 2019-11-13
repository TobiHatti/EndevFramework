using EndevFrameworkNetworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ILE = EndevFrameworkNetworkCore.InstructionLibraryExtension;
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

            server.SetAuthenticationTool(AuthenticationTools.DebugAuth);

            server.Start();

            // The server can send to a range of connected clients, wich can be selected in the server.ConnectedClients-Property

            Thread.Sleep(2000);

            server.UserGroups.NewGroup("Awesome dudes");
            server.UserGroups["Awesome dudes"].AddUser("Tobias");
            server.UserGroups["Awesome dudes"].AddUser("Adam");

            server.UserGroups.Load(@"C:\Users\zivi\Desktop\test.dat");

            while (true)
            {
                //server.Send(new ILE.TestSample(server, server.ConnectedClients[new Random().Next(0, server.ConnectedClients.Count)]));

                //Thread.Sleep(1000);

                //server.GroupSend(new ILE.RichMessageBox(server, null as NetComUser,"Hallo", "Caption", MessageBoxButtons.OK, MessageBoxIcon.Error),server.UserGroups["Awesome dudes"]);
                
                server.Broadcast(new ILE.NofityIcon(server, null, "I bin da text", "I bin da titel", 2000, ToolTipIcon.Warning));

                //server.GroupSend(new ILE.TestSample(server, null), server.UserGroups["Awesome dudes"]);

                Thread.Sleep(10000);
            }

#pragma warning disable 0162
            Console.ReadKey();
#pragma warning restore 0162
        }
    }
}
