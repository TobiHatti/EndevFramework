using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//    ____        __           ____                                   __
//   / __/__  ___/ /__ _  __  / __/___ __ ___ _  ___ _ __ __ __  ____/ /__
//  / _// _ \/ _  / -_) |/ / / _// __/ _ `/  ' \/ -_) |/|/ / _ \/ __/  '_/
// /___/_//_/\_,_/\__/|___/ /_/ /_/  \_,_/_/_/_/\__/|__,__/\___/_/ /_/\_\ 

namespace EndevFramework.NetworkCore
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
        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 1b │ F I E L D S   ( P R O T E C T E D )                    ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝    

        #region ═╣ F I E L D S   ( P R O T E C T E D ) ╠═ 

        protected bool authenticated = false;

        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 1c │ D E L E G A T E S                                      ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝    

        #region ═╣ D E L E G A T E S ╠═ 

        public delegate bool AuthenticationTool(string pUsername, string pPassword);

        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 2b │ P R O P E R T I E S   ( P U B L I C )                  ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝  

        #region ═╣ P R O P E R T I E S   ( P U B L I C ) ╠═ 

        public static AuthenticationTool AuthLookup { get; set; } = null;
        public bool Authenticated { get => authenticated; }

        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 4d │ M E T H O D S   ( P U B L I C )                        ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝ 

        #region ═╣ M E T H O D S   ( P U B L I C ) ╠═ 

        /// <summary>
        /// Authenticates the user when given a username and password.
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
            else throw new NetComAuthenticationException("*** The Authentication-Method has not been set in NetComCData ***");

            return authenticated;
        }

        #endregion
    }
}
