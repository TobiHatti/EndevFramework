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

            NetComClient client = new NetComClient(Dns.GetHostAddresses("endev.ddns.net")[0].ToString(), 2225);

            client.SetDebugOutput(DebugOutput.ToConsole);

            client.Start();

            while(true)
            {
                client.Send(new InstructionLibraryEssentials.MyStabilityTest(client, null));
                Thread.Sleep(new Random().Next(300, 3000));
            }
            

            Console.WriteLine("Done.");

            Console.ReadKey();
        }
    }
}
