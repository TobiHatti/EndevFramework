using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SampleStartupAssist
{
    class Startup
    {
#pragma warning disable IDE0060 // unused arguments
        static void Main(string[] args)
        {
            var clientPath = @"..\..\..\SampleAppClient\bin\Debug\SampleAppClient.exe";
            var serverPath = @"..\..\..\SampleAppServer\bin\Debug\SampleAppServer.exe";

            //Process.Start(serverPath);


            while (true)
            {
                Console.Write("Amount of clients: ");
                var clientCount = Convert.ToInt32(Console.ReadLine());

                for (int i = 0; i < clientCount; i++)
                {
                    Process.Start(clientPath, $"{i + 1}");
                    Thread.Sleep(500);
                }
            }

#pragma warning disable 0162 // unreachable code
            Environment.Exit(0);
#pragma warning restore 0162 // unreachable code
        }
#pragma warning restore IDE0060 // unused arguments
    }
}
