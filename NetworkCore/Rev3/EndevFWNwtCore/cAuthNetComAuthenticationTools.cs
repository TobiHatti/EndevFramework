using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndevFrameworkNetworkCore
{
    public class AuthenticationTools
    {
        public static bool DebugAuth(string pUsername, string pPassword)
        {
            switch(pUsername.ToLower())
            {
                case "tobias":
                    if (pPassword == "1") return true;
                    return false;
                case "adam":
                    if (pPassword == "2") return true;
                    return false;
                case "andrea":
                    if (pPassword == "3") return true;
                    return false;
                case "christian":
                    if (pPassword == "4") return true;
                    return false;
                default:
                    return false;
            }
        }
    }
}
