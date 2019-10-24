using EndevFWNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NCILib = EndevFWNetCore.NetComInstructionLib;

namespace SampleAppClient
{
    class Client
    {
        static void Main(string[] args)
        {
            Type t = Type.GetType(typeof(NetComInstructionLib.PlainText).AssemblyQualifiedName);
            Console.WriteLine();

            NetComClient client = new NetComClient("127.0.0.1", 2225, "TobiHatti", "Apfel123");

            client.Debug = NetComDebugOutput.ToConsole;
            client.ParseMessage = NetComMessageParser.DefaultClient;
            client.EncodeMessage = NetComMessageEncoder.DefaultClient;

            client.Start();

            int i = 0;
            while (true)
            {
                client.SendRSA(new NCILib.PlainText(client, $"Hallo i bin a test-Message N° {i++}"));
                Thread.Sleep(2222);
            }
        }
    }
}
