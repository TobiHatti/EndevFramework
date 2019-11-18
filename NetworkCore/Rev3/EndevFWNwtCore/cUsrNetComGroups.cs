using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EndevFrameworkNetworkCore
{
    /// <summary>
    /// =====================================   <para />
    /// FRAMEWORK: EndevFrameworkNetworkCore    <para />
    /// SUB-PACKAGE: User-Objects               <para />
    /// =====================================   <para />
    /// DESCRIPTION:                            <para />
    /// Groups users into managable groups.
    /// Provides a basic manager for handling 
    /// groups
    /// </summary>
    public class NetComGroups : IEnumerable
    {
        private readonly List<UserGroup> userGroups = new List<UserGroup>();

        /// <summary>
        /// Returns a UserGroup by the group-index.
        /// </summary>
        /// <param name="idx">Index of the group</param>
        /// <returns>UserGroup with fitting index</returns>
        public UserGroup this[int idx]
        {
            get
            {
                if (userGroups.Count > idx) return userGroups[idx];
                else return null;
            }
        }

        /// <summary>
        /// Returns a UserGroup by the group-name.
        /// </summary>
        /// <param name="pGroupName">Name of the group</param>
        /// <returns>UserGroup with fitting name</returns>
        public UserGroup this[string pGroupName]
        {
            get
            {
                foreach (UserGroup group in userGroups)
                    if (group.Name == pGroupName) return group;
                return null;
            }
        }

        /// <summary>
        /// Saves the usergroup-config to a file.
        /// </summary>
        /// <param name="pPath">Target file</param>
        public void Save(string pPath)
        {
            StreamWriter sw = new StreamWriter(pPath);

            foreach(UserGroup group in userGroups)
            {
                sw.WriteLine($":{group.Name}");

                foreach(string user in group.GroupMembers)
                {
                    sw.WriteLine($"!{user.ToLower()}");
                }
            }

            sw.Close();
        }

        /// <summary>
        /// Loads the usergroup-config from a file.
        /// </summary>
        /// <param name="pPath">Target file</param>
        public void Load(string pPath)
        {
            using (StreamReader sr = new StreamReader(pPath))
            {
                string line;
                string currentLine = "";
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith(":"))
                    {
                        userGroups.Add(new UserGroup(line.Remove(0, 1)));
                        currentLine = line.Remove(0, 1);
                    }

                    if (line.StartsWith("!"))
                    {
                        this[currentLine].AddUser(line.Remove(0, 1));
                    }
                }
            }

        }

        /// <summary>
        /// Disconnects a user from any groups he is currently connected to.
        /// </summary>
        /// <param name="pUserSocket">Socket of the user</param>
        public void Disconnect(Socket pUserSocket)
        {
            foreach (UserGroup group in userGroups)
                for(int i = 0; i < group.OnlineMembers.Count; i++)
                    if (group.OnlineMembers[i].LocalSocket == pUserSocket)
                    {
                        group.OnlineMembers.Remove(group.OnlineMembers[i]);
                        //yield return group.Name;
                    }
        }

        /// <summary>
        /// Creates a new group with members (optional).
        /// </summary>
        /// <param name="pGroupName">Name of the group</param>
        /// <param name="pUsers">Users to add to the group</param>
        public void NewGroup(string pGroupName, params NetComUser[] pUsers)
        {
            userGroups.Add(new UserGroup(pGroupName));

            foreach(NetComUser user in pUsers)
                this[pGroupName].AddUser(user);
        }

        /// <summary>
        /// Tries to add a user to any groups 
        /// that that it is a member of. 
        /// </summary>
        /// <param name="pUser"></param>
        public void TryGroupAdd(NetComUser pUser)
        {
            for (int i = 0; i < userGroups.Count; i++)
                for(int j = 0; j < userGroups[i].GroupMembers.Count; j++)
                    if(pUser.Username.ToLower() == userGroups[i].GroupMembers[j].ToLower())
                    {
                        userGroups[i].AddUser(pUser);
                    }
        }

        /// <summary>
        /// Returns all UserGroups.
        /// Required for IEnumerable-Interface.
        /// </summary>
        /// <returns>UserGroups enumerator</returns>
        public IEnumerator GetEnumerator()
        {
            foreach (UserGroup group in userGroups)
                yield return group;
        }
    }
}
