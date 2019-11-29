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
    /// SUB-PACKAGE: Encoding-Handlers          <para />
    /// =====================================   <para />
    /// DESCRIPTION:                            <para />
    /// Encodes / Decodes Base64-Strings
    /// </summary>
    public class Base64Handler
    {
        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 4d │ M E T H O D S   ( P U B L I C )                        ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝ 

        #region ═╣ M E T H O D S   ( P U B L I C ) ╠═ 

        /// <summary>
        /// Converts a string into a 
        /// Base64-encoded string.
        /// </summary>
        /// <param name="pPlainText">Plain text to be encoded</param>
        /// <returns>Base64-Encoded string</returns>
        public static string Encode(string pPlainText)
            => Convert.ToBase64String(Encoding.UTF8.GetBytes(pPlainText));

        /// <summary>
        /// Decodes a base64-encoded string into
        /// a readable plain-text string.
        /// </summary>
        /// <param name="pBase64String">Base64 string to be decoded</param>
        /// <param name="pOnErrorReturn">Return-Value in case an error occurs. Null will throw an exception.</param>
        /// <returns>Plain text string</returns>
        public static string Decode(string pBase64String, string pOnErrorReturn = null)
        {
            if (pOnErrorReturn == null) return Encoding.UTF8.GetString(Convert.FromBase64String(pBase64String));
            else
            {
                try { return Encoding.UTF8.GetString(Convert.FromBase64String(pBase64String)); }
                catch { return pOnErrorReturn; }
            }
        }

        #endregion
    }
}
