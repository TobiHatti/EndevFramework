using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndevFrameworkNetworkCore
{
    public class UserGroup
    {
        public string Name { get; private set; } = null;

        internal List<NetComUser> OnlineMembers { get; } = new List<NetComUser>();
        internal List<string> GroupMembers { get; } = new List<string>();

        public UserGroup(string pGroupName)
        {
            Name = pGroupName;
        }

        
        public void AddUser(NetComUser pUser)
        {
            if (!OnlineMembers.Contains(pUser)) OnlineMembers.Add(pUser);
            if (!GroupMembers.Contains(pUser.Username)) GroupMembers.Add(pUser.Username);
        }

        public void AddUser(string pUsername)
        {
            if(!GroupMembers.Contains(pUsername)) GroupMembers.Add(pUsername);
        }

        public void Disconnect(NetComUser pUser)
        {
            if (OnlineMembers.Contains(pUser)) OnlineMembers.Remove(pUser);
        }

        public void Remove(string pUsername)
        {
            if (!GroupMembers.Contains(pUsername)) GroupMembers.Add(pUsername);
        }
    }
}
