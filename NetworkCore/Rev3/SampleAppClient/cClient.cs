using EndevFWNwtCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SampleAppClient
{
    class Client
    {
        static void Main(string[] args)
        {
            var clientNr = "0";
            try
            {
                clientNr = args[0];
            }
            catch
            {

            }

            Console.Title = $"Client {clientNr}";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("===================================");
            Console.WriteLine($"=           C L I E N T - {clientNr}       =");
            Console.WriteLine("===================================\r\n");

            Console.WriteLine(Dns.GetHostAddresses("endev.ddns.net")[0].ToString());

            //NetComClient client = new NetComClient(Dns.GetHostAddresses("endev.ddns.net")[0].ToString(), 2225);
            NetComClient client = new NetComClient("127.0.0.1", 2225);

            

            Console.Write("Username: ");
            string username = Console.ReadLine();

            Console.Write("Password: ");
            string password = Console.ReadLine();

            client.Login(username, password);

            client.SetDebugOutput(DebugOutput.ToConsole);

            client.Start();

            Thread.Sleep(3000);

            while (true)
            {
                // Clients can only send directly to the server, so the receiver is set to null
                client.Send(new InstructionLibraryEssentials.MyStabilityTest(client, null));
                Thread.Sleep(new Random().Next(300, 3000));
            }
            

            Console.WriteLine("Done.");

            Console.ReadKey();
        }
    }
}
