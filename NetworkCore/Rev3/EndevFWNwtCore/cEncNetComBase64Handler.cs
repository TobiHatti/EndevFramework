using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Segment Complete [Last Modified 30.10.2019]

namespace EndevFWNwtCore
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
        /// a readable plain-text string
        /// </summary>
        /// <param name="pBase64String">Base64 string to be decoded</param>
        /// <returns>Plain text string</returns>
        public static string Decode(string pBase64String) 
            => Encoding.UTF8.GetString(Convert.FromBase64String(pBase64String));
    }
}
