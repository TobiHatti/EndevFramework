using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndevFWNwtCore
{
    public class AuthenticationTools
    {
        public static bool DebugAuth(string pUsername, string pPassword)
        {
            switch(pUsername)
            {
                case "Tobias":
                    if (pPassword == "1") return true;
                    return false;
                case "Adam":
                    if (pPassword == "2") return true;
                    return false;
                case "Andrea":
                    if (pPassword == "3") return true;
                    return false;
                case "Christian":
                    if (pPassword == "4") return true;
                    return false;
                default:
                    return false;
            }
        }
    }
}
