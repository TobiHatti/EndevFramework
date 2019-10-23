﻿using EndevFWNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SampleAppClient
{
    class Client
    {
        static void Main(string[] args)
        {


            NetComClient client = new NetComClient("127.0.0.1", 2225, "TobiHatti", "Apfel123");

            client.Debug = NetComDebugOutput.ToConsole;
            client.ParseMessage = NetComMessageParser.DefaultClient;
            client.EncodeMessage = NetComMessageEncoder.DefaultClient;
            client.LibraryExec = NetComLibraryExecuter.DefaultClient;

            client.Start();

            int i = 0;
            while (true)
            {
                client.SendRSA($"Hallo i bin a test-Message N° {i++}");
                Thread.Sleep(2222);
            }
        }
    }
}
