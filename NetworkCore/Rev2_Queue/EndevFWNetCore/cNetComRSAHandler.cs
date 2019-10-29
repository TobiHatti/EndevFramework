using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EndevFWNetCore
{
    public class NetComRSAHandler
    {
        private UnicodeEncoding encoder = new UnicodeEncoding();
        private RSACryptoServiceProvider RSA = null;

        private string PrivateKey = null;
        public string PublicKey { get; private set; } = null;

        public NetComRSAHandler()
        {
            RSA = new RSACryptoServiceProvider();
            PublicKey = RSA.ToXmlString(false);
            PrivateKey = RSA.ToXmlString(true);
        }


        public string Decrypt(string data)
        {
            var rsa = new RSACryptoServiceProvider();
            var dataArray = data.Split(new char[] { '-' });
            byte[] dataByte = new byte[dataArray.Length];
            for (int i = 0; i < dataArray.Length; i++)
            {
                dataByte[i] = Convert.ToByte(dataArray[i]);
            }

            rsa.FromXmlString(PrivateKey);
            var decryptedByte = rsa.Decrypt(dataByte, false);
            return Encoding.Unicode.GetString(decryptedByte);
            //return encoder.GetString(decryptedByte);
        }

        public string Encrypt(string data, string pPartnerPublicKey)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(pPartnerPublicKey);
            var dataToEncrypt = Encoding.Unicode.GetBytes(data);
            //var dataToEncrypt = encoder.GetBytes(data);
            var encryptedByteArray = rsa.Encrypt(dataToEncrypt, false).ToArray();
            var length = encryptedByteArray.Count();
            var item = 0;
            var sb = new StringBuilder();
            foreach (var x in encryptedByteArray)
            {
                item++;
                sb.Append(x);

                if (item < length)
                    sb.Append("-");
            }
            return sb.ToString();
        }
    }
}
