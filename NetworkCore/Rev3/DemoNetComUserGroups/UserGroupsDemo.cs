using EndevFrameworkNetworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNetComUserGroups
{
    class UserGroupsDemo
    {
        static void Main(string[] args)
        {
            // UserGroups can be used to manage clients that are connected (and not connected) 
            // to the server, by putting them into groups.
            // A single client can be assigned to any number of groups.

            // Create the server
            NetComServer server = new NetComServer(2225);
            server.SetDebugOutput(DebugOutput.ToConsole);
            server.SetAuthenticationTool(AuthenticationTools.FullAllow);

            // Load previously created groups and their users into memory.
            server.UserGroups.Load(@"Path\To\Your\Config\File");
            
            // Start the server
            server.Start();

            // Create a new user-group (if it doesn't existent yet)
            server.UserGroups.NewGroup("SampleUserGroup");

            // Add users to the user-group
            // Users can be added if they are online or offline with the username. 
            // No matter how they get added the first time, they will allways get reassigned
            // to this group when they connect unless they get removed from the group.

            // Adding a already connected user
            server.UserGroups["SampleUserGroup"].AddUser(server.ConnectedClients[6]);

            // Adding a user by its username
            server.UserGroups["SampleUserGroup"].AddUser("SomeUsername");

            // By using the Disconnect-Method, the user gets excluded from the group until
            // he reconnects. This does not permanently remove the user from the group.
            server.UserGroups["SampleUserGroup"].Disconnect(server.ConnectedClients[2]);

            // To completely remove a user from a group, use the Remove-Method.
            // When removing a user, it stays connected until it disconnects, but it will not get
            // re-assigned to the group again.
            server.UserGroups["SampleUserGroup"].Remove("AnotherUsername");

            // To save the group-configuration to a file, use the Save-Method
            server.UserGroups.Save(@"Path\To\Your\Config\File");
        }
    }
}
