using EndevFWNwtCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

            NetComClient client = new NetComClient("127.0.0.1", 2225);

            client.SetDebugOutput(DebugOutput.ToConsole);

            client.Start();

            while(true)
            {
                client.Send(new InstructionLibraryEssentials.MySampleInstruction(client, null, "Hallo"));
                Thread.Sleep(new Random().Next(300, 10000));
            }
            

            Console.WriteLine("Done.");

            Console.ReadKey();
        }
    }
}
