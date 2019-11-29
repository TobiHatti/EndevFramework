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
    /// SUB-PACKAGE: User-Objects               <para />
    /// =====================================   <para />
    /// DESCRIPTION:                            <para />
    /// Single user-group containing 
    /// group-members and online-members.
    /// Used by the NetComGroups-Class
    /// </summary>
    public class UserGroup
    {
        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 2a │ P R O P E R T I E S   ( I N T E R N A L )              ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝  

        #region ═╣ P R O P E R T I E S   ( I N T E R N A L ) ╠═ 

        internal List<NetComUser> OnlineMembers { get; } = new List<NetComUser>();
        internal List<string> GroupMembers { get; } = new List<string>();

        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 2b │ P R O P E R T I E S   ( P U B L I C )                  ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝  

        #region ═╣ P R O P E R T I E S   ( P U B L I C ) ╠═

        public string Name { get; private set; } = null;

        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 3  │ C O N S T R U C T O R S                                ║
        // ╚════╧════════════════════════════════════════════════════════╝  

        #region ═╣ C O N S T R U C T O R S ╠═ 

        /// <summary>
        /// Creates a new group with a custom name.
        /// </summary>
        /// <param name="pGroupName">Name of the new group</param>
        public UserGroup(string pGroupName)
        {
            Name = pGroupName;
        }

        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 4d │ M E T H O D S   ( P U B L I C )                        ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝ 

        #region ═╣ M E T H O D S   ( P U B L I C ) ╠═ 

        /// <summary>
        /// Adds a already connected user to the group.
        /// The user reconnects at the next session.
        /// </summary>
        /// <param name="pUser">Client</param>
        public void AddUser(NetComUser pUser)
        {
            if (!OnlineMembers.Contains(pUser)) OnlineMembers.Add(pUser);
            if (!GroupMembers.Contains(pUser.Username.ToLower())) GroupMembers.Add(pUser.Username.ToLower());
        }

        /// <summary>
        /// Adds a user to a group.
        /// User connects to the group at the next session.
        /// </summary>
        /// <param name="pUsername">Client's username</param>
        public void AddUser(string pUsername)
        {
            if (!GroupMembers.Contains(pUsername.ToLower())) GroupMembers.Add(pUsername.ToLower());
        }

        /// <summary>
        /// Removes a connected user from the group.
        /// The user reconnects at the next session.
        /// </summary>
        /// <param name="pUsername">Client</param>
        public void Disconnect(NetComUser pUser)
        {
            if (OnlineMembers.Contains(pUser)) OnlineMembers.Remove(pUser);
        }

        /// <summary>
        /// Removes a user from the group.
        /// The user stays connected until the next session.
        /// </summary>
        /// <param name="pUsername">Client's username</param>
        public void Remove(string pUsername)
        {
            if (GroupMembers.Contains(pUsername.ToLower())) GroupMembers.Remove(pUsername.ToLower());
        }

        #endregion
    }
}
