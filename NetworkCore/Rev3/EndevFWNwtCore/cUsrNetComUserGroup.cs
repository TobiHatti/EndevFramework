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

        private List<NetComUser> LUsers = new List<NetComUser>();

        public UserGroup(string pGroupName)
        {
            Name = pGroupName;
        }

        public void AddUser(NetComUser pUser)
        {

        }

        public void AddUser(string pUsername)
        {

        }
    }
}
