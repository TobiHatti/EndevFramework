using EndevFrameworkNetworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetComServerInstruction
{
    class Program
    {
        static void Main(string[] args)
        {
            // === SETTING UP THE SERVER ===

            // Define the tcp-port on which the communication should take place
            int tcpServerPort = 2225;

            // Create a instance of the server
            NetComServer server = new NetComServer(tcpServerPort);

            // Set the wanted Debug-Output
            server.SetDebugOutput(DebugOutput.ToConsole);

            // Set the authentication-method.
            // Authentication-Methods can be customly created, as long as 
            // they accept a username [string] and a password [string] and 
            // return true or false wether the authentication / login was successfull or not
            server.SetAuthenticationTool(AuthenticationTools.DebugAuth);

            // Start the server and all its background-processes.
            server.Start();


            // === COMMUNICATE WITH CLIENTS ===

            // Create the instruction you want to send. General purpose-instructions are defined in the 
            // InstructionLibraryEssentials-class. Additional Instructions are defined in the 
            // InstructionLibraryExtensions-class. Custom instructions can be created as shown in the 
            // "CustomInstructions"-Project

            // 1) Send a message to a single connected client

            // Connected clients can be selected using the server.ConnectedClients-Property.

            InstructionBase instruction1 = new InstructionLibraryEssentials.SimpleMessageBox
                (server, server.ConnectedClients[0], "Hello world.");

            // As soon as server.Send(...) gets called, the instruction is queued for sending and gets 
            // sent out as soon as the instruction-preprocessing is done.
            server.Send(instruction1);

            // 2) Send a message to all connected clients (Broadcast)

            // When creating a broadcast-message, the receiver-argument gets set to 'null'

            InstructionBase instruction2 = new InstructionLibraryEssentials.SimpleMessageBox
                (server, null, "Hello world.");

            server.Broadcast(instruction2);

            // 3) Send a message to a list of clients 

            // When creating a list-message, the receiver-argument gets set to 'null'

            InstructionBase instruction3 = new InstructionLibraryEssentials.SimpleMessageBox
                (server, null, "Hello world.");

            server.ListSend(instruction3, server.ConnectedClients[0], server.ConnectedClients["SampleUser01"]);

            // 4) Send a message to a pre-defined group of clients 

            // Create a new user-group if not existent yet
            server.UserGroups.NewGroup("SampleUserGroup");

            // Add users to the user-group
            server.UserGroups["SampleUserGroup"].AddUser(server.ConnectedClients[0]);
            server.UserGroups["SampleUserGroup"].AddUser(server.ConnectedClients[6]);
            server.UserGroups["SampleUserGroup"].AddUser("SomeUsername");

            // Users can be directly added to a group when they are connected, 
            //or they can be added using a username and get assigned to the group as soon as they connect.

            // To save and load groups from a file, use the NetcomGroups.Load-Method (before server.Start()) and 
            // the NetComGroups.Save()-Method

            // When creating a group-message, the receiver-argument gets set to 'null'

            InstructionBase instruction4 = new InstructionLibraryEssentials.SimpleMessageBox
                (server, null, "Hello world.");

            server.GroupSend(instruction4, server.UserGroups["SampleUserGroup"]);


        }
    }
}
