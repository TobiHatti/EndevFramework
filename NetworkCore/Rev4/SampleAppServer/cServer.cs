using EndevFrameworkNetworkCore;
using System;
using System.Linq;
using System.Threading;
using ILE = EndevFrameworkNetworkCore.InstructionLibraryEssentials;

namespace SampleAppServer
{
    class Server
    {
#pragma warning disable IDE0060 // unused parameters
        static void Main(string[] args)
        {
            Console.Title = "Server";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("===================================");
            Console.WriteLine("=            S E R V E R          =");
            Console.WriteLine("===================================\r\n");
            Console.ForegroundColor = ConsoleColor.White;


            //NetComServerHandler serverHandler = new NetComServerHandler(2225);
            //NetComServer server = serverHandler.GetServer();

            ServerHandler serverHandler = new ServerHandler(2225);
            
            serverHandler.SetDebugOutput(DebugOutput.ToConsole);

            serverHandler.SetAuthenticationTool(AuthenticationTools.DebugAuth);

            serverHandler.Start();

            // The server can send to a range of connected clients, wich can be selected in the server.ConnectedClients-Property

            Thread.Sleep(2000);

            NetComServer server = serverHandler.GetServer();

            server.UserGroups.NewGroup("Awesome dudes");
            server.UserGroups["Awesome dudes"].AddUser("Tobias");
            server.UserGroups["Awesome dudes"].AddUser("Adam");

            //server.UserGroups.Load(@"C:\Users\zivi\Desktop\test.dat");

            while (true)
            {
                Console.Title = $"Server  -  Send: {serverHandler.HandlerData.LogSendCounter}     Receive: {serverHandler.HandlerData.LogReceiveCounter}";
                server.Send(new ILE.TestSample(server, server.ConnectedClients[new Random().Next(0, server.ConnectedClients.Count)]));

                Thread.Sleep(2000);

                Console.Title = $"Server  -  Send: {serverHandler.HandlerData.LogSendCounter}     Receive: {serverHandler.HandlerData.LogReceiveCounter}";
                server.GroupSend(new ILE.TestSample(server, null),server.UserGroups["Awesome dudes"]);

                Thread.Sleep(2000);

                Console.Title = $"Server  -  Send: {serverHandler.HandlerData.LogSendCounter}     Receive: {serverHandler.HandlerData.LogReceiveCounter}";
                server.Broadcast(new ILE.TestSample(server, null));

                Thread.Sleep(2000);
            }

      

#pragma warning disable 0162 // unreachable code
            Console.ReadKey();
#pragma warning restore 0162 // unreachable code
        }
#pragma warning restore IDE0060 // unused arguments
    }
}
