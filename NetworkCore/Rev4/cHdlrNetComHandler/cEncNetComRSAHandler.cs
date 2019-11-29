using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EndevFrameworkNetworkCore
{
    // ╔════╤════════════════════════════════════════════════════════╗
    // ║ 7  │ S T R U C T S                                          ║
    // ╚════╧════════════════════════════════════════════════════════╝  

    #region ═╣ S T R U C T S ╠═ 

    /// <summary>
    /// Contains the public and the 
    /// private key of an user
    /// </summary>
    public struct RSAKeyPair
    {
        public string PrivateKey;
        public string PublicKey;
    }

    #endregion

    /// <summary>
    /// =====================================   <para />
    /// FRAMEWORK: EndevFrameworkNetworkCore    <para />
    /// SUB-PACKAGE: Encoding-Handlers          <para />
    /// =====================================   <para />
    /// DESCRIPTION:                            <para />
    /// Encrypts and Decrypts RSA-Messages.     <para />
    /// Provides Public and Private keys for
    /// asynchronous encryption.
    /// </summary>
    public class RSAHandler
    {
        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 1a │ F I E L D S   ( P R I V A T E )                        ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝    

        #region ═╣ F I E L D S   ( P R I V A T E ) ╠═ 

        private static readonly char RSAByteDelimiter = '-';
        private static readonly UnicodeEncoding encoder = new UnicodeEncoding();

        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 4d │ M E T H O D S   ( P U B L I C )                        ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝ 

        #region ═╣ M E T H O D S   ( P U B L I C ) ╠═ 

        /// <summary>
        /// Generates a unique key-pair for RSA-encryption.
        /// </summary>
        /// <returns>A Private/Public Key-Pair</returns>
        public static RSAKeyPair GenerateKeyPair()
        {
            RSAKeyPair keys = new RSAKeyPair();
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                keys.PublicKey = RSA.ToXmlString(false);
                keys.PrivateKey = RSA.ToXmlString(true);
            }

            return keys;
        }

        /// <summary>
        /// Encrypts a string using a public RSA-key.
        /// </summary>
        /// <param name="pPartnerPublicKey">Public key of the recepiant</param>
        /// <param name="pData">Data to be encrypted</param>
        /// <returns>The encrypted data-string</returns>
        public static string Encrypt(string pPartnerPublicKey, string pData)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(pPartnerPublicKey);
                byte[] dataToEncrypt = encoder.GetBytes(pData);
                byte[] encryptedByteArray = rsa.Encrypt(dataToEncrypt, false).ToArray();
                int length = encryptedByteArray.Count();
                int item = 0;
                StringBuilder sb = new StringBuilder();
                foreach (byte x in encryptedByteArray)
                {
                    item++;
                    sb.Append(x);
                    if (item < length) sb.Append(RSAByteDelimiter);
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Decrypts a RSA-encrypted string using a private-key.
        /// </summary>
        /// <param name="pLocalPrivateKey">Private-key of the user (recepiant)</param>
        /// <param name="pData">RSA-encrypted data-string</param>
        /// <returns>The decrypted string</returns>
        public static string Decrypt(string pLocalPrivateKey, string pData)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                string[] dataArray = pData.Split(new char[] { RSAByteDelimiter });
                byte[] dataByte = new byte[dataArray.Length];
                for (int i = 0; i < dataArray.Length; i++)
                {
                    dataByte[i] = Convert.ToByte(dataArray[i]);
                }
                rsa.FromXmlString(pLocalPrivateKey);
                byte[] decryptedByte = rsa.Decrypt(dataByte, false);
                return encoder.GetString(decryptedByte);
            }
        }

        /// <summary>
        /// Creates a SHA256-Based signature using the senders private key.
        /// </summary>
        /// <param name="pLocalPrivateKey">Private-Key of the sender</param>
        /// <param name="pData">String used for the signature</param>
        /// <returns>The encrypted data-String</returns>
        public static string Sign(string pLocalPrivateKey, string pData)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(pLocalPrivateKey);
                byte[] dataToEncrypt = encoder.GetBytes(pData);
                byte[] encryptedByteArray = rsa.SignData(dataToEncrypt, new SHA256CryptoServiceProvider()).ToArray();
                int length = encryptedByteArray.Count();
                int item = 0;
                StringBuilder sb = new StringBuilder();
                foreach (byte x in encryptedByteArray)
                {
                    item++;
                    sb.Append(x);
                    if (item < length) sb.Append(RSAByteDelimiter);
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Checks if the signature of the partner is valid.
        /// </summary>
        /// <param name="pPartnerPublicKey">Public-Key of the user (sender)</param>
        /// <param name="pOriginalMessage">Original, unencrypted signature</param>
        /// <param name="pSignedMessage">RSA-Encrypted Signature</param>
        /// <returns></returns>
        public static bool Verify(string pPartnerPublicKey, string pOriginalMessage, string pSignedMessage)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                string[] dataArray = pSignedMessage.Split(new char[] { RSAByteDelimiter });
                byte[] dataByte = new byte[dataArray.Length];
                for (int i = 0; i < dataArray.Length; i++)
                {
                    dataByte[i] = Convert.ToByte(dataArray[i]);
                }
                rsa.FromXmlString(pPartnerPublicKey);

                return rsa.VerifyData(encoder.GetBytes(pOriginalMessage), new SHA256CryptoServiceProvider(), dataByte);
            }
        }

        #endregion
    }
}
