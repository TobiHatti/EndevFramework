using EndevFrameworkNetworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetComClientDemo
{
    class ClientDemo
    {
        static void Main(string[] args)
        {
            #region ========= SETTING UP THE CLIENT =========

            // Set the server-IP
            // If your server and client are running on the same machine, use '127.0.0.1'
            // If you need to resolve an URL, use 'Dns.GetHostAddresses("your.sample-url.com")[0].ToString()'
            string serverIP = "212.169.76.12";

            // Set the TCP-Port on which the server is hosted
            int tcpServerPort = 2225;

            // Create a instance of the server
            NetComClient client = new NetComClient(serverIP, tcpServerPort);

            // Enter the username and password of the client.
            // In case the authentication fails, the instructions won't get 
            // processed on the server.
            client.Login("SampleUsername", "SamplePassword");

            // Set the wanted Debug-Output
            // Pre-Defined debug-outputs can be found
            // in the DebugOutput-Class
            client.SetDebugOutput(DebugOutput.ToConsole);

            // Start the client and all its background-processes.
            client.Start();

            #endregion

            #region ========= COMMUNICATE WITH CLIENTS =========

            // Create the instruction that should be sent.
            // Since the only receiver a client can address is the server, the receiver-parameter is always 'null'
            var instruction = new InstructionLibraryEssentials.SimpleMessageBox(client, null, "Hello world!");

            // As soon as server.Send(...) gets called, the instruction is queued for sending and gets 
            // sent out as soon as the instruction-preprocessing is done.
            client.Send(instruction);

            #endregion
        }
    }
}
