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
    public class NetComGroups : IEnumerable
    {
        private readonly List<UserGroup> userGroups = new List<UserGroup>();

        public UserGroup this[int idx]
        {
            get
            {
                if (userGroups.Count > idx) return userGroups[idx];
                else return null;
            }
        }

        public UserGroup this[string pGroupName]
        {
            get
            {
                foreach (UserGroup group in userGroups)
                    if (group.Name == pGroupName) return group;
                return null;
            }
        }

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

        public void Load(string pPath)
        {
            StreamReader sr = new StreamReader(pPath);
            string line;
            string currentLine = "";
            while ((line = sr.ReadLine()) != null)
            {
                if(line.StartsWith(":"))
                {
                    userGroups.Add(new UserGroup(line.Remove(0, 1)));
                    currentLine = line.Remove(0, 1);
                }

                if(line.StartsWith("!"))
                {
                    this[currentLine].AddUser(line.Remove(0, 1));
                }
            }

        }

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

        public void NewGroup(string pGroupName, params NetComUser[] pUsers)
        {
            userGroups.Add(new UserGroup(pGroupName));
        }

        public void TryGroupAdd(NetComUser pUser)
        {
            foreach (UserGroup group in userGroups)
                foreach (string username in group.GroupMembers)
                    if (pUser.Username.ToLower() == username.ToLower())
                    {
                        group.AddUser(pUser);
                        //yield return group.Name;
                    }
        }

        public IEnumerator GetEnumerator()
        {
            foreach (UserGroup group in userGroups)
                yield return group;
        }
    }
}
