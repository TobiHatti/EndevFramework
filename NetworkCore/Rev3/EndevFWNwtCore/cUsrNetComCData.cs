using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndevFWNwtCore
{
    /// <summary>
    /// =====================================   <para />
    /// FRAMEWORK: EndevFrameworkNetworkCore    <para />
    /// SUB-PACKAGE: User-Objects               <para />
    /// =====================================   <para />
    /// DESCRIPTION:                            <para />
    /// Stores all necesary informations for a Server
    /// to communicate with the client.
    /// </summary>
    public class NetComCData : NetComUser
    {
        public delegate bool AuthenticationTool(string pUsername, string pPassword);
        private AuthenticationTool AuthLookup = null;



        protected bool authenticated = false;

        public void SetAuthenticationTool(AuthenticationTool pLookupTool)
        {
            AuthLookup = pLookupTool;
        }

        /// <summary>
        /// Authenticates the user when given a username and password
        /// </summary>
        /// <param name="pPassword">Users Password</param>
        /// <param name="pUsername">Users Username</param>
        /// <returns>True if the authentication was sucessfull</returns>
        public bool Authenticate(string pPassword, string pUsername = null)
        {
            if (Password == pPassword && Username == pUsername && authenticated) return true;

            if (Password != pPassword) authenticated = false;

            if (pUsername != null && Username != pUsername) authenticated = false; 

            if (pUsername != null) Username = pUsername;

            Password = pPassword;

            if (Password == null || Username == null) return false;

            if (AuthLookup != null) authenticated = AuthLookup(Username, Password);
            else throw new NetComAuthenticationException("*** The Authentication-Method has not been setin NetComCData ***");

           

            return authenticated;
        }


    }
}
