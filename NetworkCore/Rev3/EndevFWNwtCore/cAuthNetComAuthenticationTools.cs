using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndevFrameworkNetworkCore
{
    /// <summary>
    /// =====================================   <para />
    /// FRAMEWORK: EndevFrameworkNetworkCore    <para />
    /// SUB-PACKAGE: Authentication-Tools       <para />
    /// =====================================   <para />
    /// DESCRIPTION:                            <para />
    /// Provides several tools to authenticate
    /// clients on the server
    /// </summary>
    public class AuthenticationTools
    {
#pragma warning disable IDE0060 // unused parameters

        /// <summary>
        /// Always denies the entered user-data.
        /// </summary>
        /// <param name="pUsername">Username</param>
        /// <param name="pPassword">Password</param>
        /// <returns>True if the authentication was successfull</returns>
        public static bool FullDeny(string pUsername, string pPassword)
            => false;

        /// <summary>
        /// Always allows the entered user-data.
        /// </summary>
        /// <param name="pUsername">Username</param>
        /// <param name="pPassword">Password</param>
        /// <returns>True if the authentication was successfull</returns>
        public static bool FullAllow(string pUsername, string pPassword)
            => true;

        /// <summary>
        /// Provides a range of test-users for development and testing.
        /// </summary>
        /// <param name="pUsername">Username</param>
        /// <param name="pPassword">Password</param>
        /// <returns>True if the authentication was successfull</returns>
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

#pragma warning restore IDE0060 // unused parameters
    }
}
