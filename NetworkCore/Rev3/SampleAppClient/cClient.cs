using EndevFWNwtCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleAppClient
{
    class Client
    {
        static void Main(string[] args)
        {
            var clientNr = args[0];

            Console.Title = $"Client {clientNr}";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("===================================");
            Console.WriteLine($"=           C L I E N T - {clientNr}       =");
            Console.WriteLine("===================================\r\n");

            NetComClient client = new NetComClient("127.0.0.1", 2225);

            client.SetDebugOutput(DebugOutput.ToConsole);

            client.Start();

            Console.ReadKey();
        }
    }
}
