using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        }

        public void Load(string pPath)
        {

        }

        public void NewGroup(string pGroupName, params NetComUser[] pUsers)
        {
            userGroups.Add(new UserGroup(pGroupName));
        }

        public IEnumerable<string> TryGroupAdd(NetComUser pUser)
        {
            foreach(UserGroup group in userGroups)
                foreach(string username in group.GroupMembers)
                    if (pUser.Username == username)
                    {
                        group.AddUser(pUser);
                        yield return group.Name;
                    }
        }

        public IEnumerator GetEnumerator()
        {
            foreach (UserGroup group in userGroups)
                yield return group;
        }
    }
}
