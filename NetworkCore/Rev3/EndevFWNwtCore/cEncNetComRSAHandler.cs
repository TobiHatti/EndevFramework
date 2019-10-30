using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

// Segment Complete [Last Modified 29.10.2019]

namespace EndevFWNwtCore
{
    /// <summary>
    /// Contains the public and the 
    /// private key of an user
    /// </summary>
    public struct RSAKeyPair
    {
        public string PrivateKey;
        public string PublicKey;
    }

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
        private static char RSAByteDelimiter = '-';

        /// <summary>
        /// Generates a unique key-pair for RSA-encryption
        /// </summary>
        /// <returns>A Private/Public Key-Pair</returns>
        public static RSAKeyPair GenerateKeyPair()
        {
            RSAKeyPair keys = new RSAKeyPair();
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            keys.PublicKey = RSA.ToXmlString(false);
            keys.PrivateKey = RSA.ToXmlString(true);

            return keys;
        }

        /// <summary>
        /// Encrypts a string using a public RSA-key
        /// </summary>
        /// <param name="pPartnerPublicKey">Public key of the recepiant</param>
        /// <param name="pData">Data to be encrypted</param>
        /// <returns>The encrypted data-string</returns>
        public static string Encrypt(string pPartnerPublicKey, string pData)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(pPartnerPublicKey);
            byte[] dataToEncrypt = Encoding.Unicode.GetBytes(pData);
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

        /// <summary>
        /// Decrypts a RSA-encrypted string using a private-key
        /// </summary>
        /// <param name="pLocalPrivateKey">Private-key of the user (recepiant)</param>
        /// <param name="pData">RSA-encrypted data-string</param>
        /// <returns>The decrypted string</returns>
        public static string Decrypt(string pLocalPrivateKey, string pData)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            string[] dataArray = pData.Split(new char[] { RSAByteDelimiter });
            byte[] dataByte = new byte[dataArray.Length];
            for (int i = 0; i < dataArray.Length; i++)
            {
                dataByte[i] = Convert.ToByte(dataArray[i]);
            }
            rsa.FromXmlString(pLocalPrivateKey);
            byte[] decryptedByte = rsa.Decrypt(dataByte, false);
            return Encoding.Unicode.GetString(decryptedByte);
        }
    }
}
