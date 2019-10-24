using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndevFWNetCore
{
    public class NetComUserDummy
    {
        public string Username { get; set; } = null;
        public string Password { get; set; } = null;
        public string PublicKey { get; set; } = null;

        public NetComUserDummy(string pUsername, string pPassword) : this(pUsername, pPassword, null) { }
        public NetComUserDummy(string pUsername, string pPassword, string pPublicKey)
        {
            Username = pUsername;
            Password = pPassword;
            PublicKey = pPublicKey;
        }
    }
}
