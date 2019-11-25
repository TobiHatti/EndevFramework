using EndevFrameworkNetworkCore;
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
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(Dns.GetHostAddresses("endev.ddns.net")[0].ToString());

            //NetComClient client = new NetComClient(Dns.GetHostAddresses("endev.ddns.net")[0].ToString(), 2225);
            NetComClient client = new NetComClient("127.0.0.1", 2225);

            

            Console.Write("Username: ");
            string username = Console.ReadLine();

            Console.Write("Password: ");
            string password = Console.ReadLine();

            client.Login(username, password);
            //client.Login("Tobias", "1");

            client.SetDebugOutput(DebugOutput.ToConsole);

            client.Start();

            Thread.Sleep(3000);
            client.Send(new InstructionLibraryEssentials.TestSample(client, null));
            while (true)
            {
                // Clients can only send directly to the server, so the receiver is set to null

                Console.Title = $"Client {clientNr}  -  Send: {client.TotalSendCounter}     Receive: {client.TotalReceiveCounter}";
                client.Send(new InstructionLibraryEssentials.TestSample(client, null));

                //client.Send(new InstructionLibraryEssentials.RichMessageBox(client, null, "Hallo", "Titel", System.Windows.Forms.MessageBoxButtons.OKCancel, System.Windows.Forms.MessageBoxIcon.Hand));
                
                //client.Send(new InstructionLibraryEssentials.ToOutputStream(client, null, "Hallo"));


                Thread.Sleep(new Random().Next(500, 1000));
            }

#pragma warning disable 0162 // unreachable code
            Console.WriteLine("Done.");

            Console.ReadKey();
#pragma warning restore 0162 // unreachable code
        }
    }
}
