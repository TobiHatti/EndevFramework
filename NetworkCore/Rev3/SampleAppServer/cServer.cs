using EndevFWNwtCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SampleAppServer
{
    class Server
    {
        static void Main(string[] args)
        {
            Console.Title = "Server";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("===================================");
            Console.WriteLine("=            S E R V E R          =");
            Console.WriteLine("===================================\r\n");

            NetComUser user = new NetComUser();
            NetComUser rec = new NetComUser();
            

            InstructionBase instr = new InstructionLibraryExtension.MySampleInstruction(user, "Hallo");

            string msgString = instr.Encode(rec);

            Console.WriteLine(msgString);


            InstructionBase[] result = InstructionOperations.Parse(msgString).ToArray();

            instr.Execute();


            RSAKeyPair user1 = RSAHandler.GenerateKeyPair();
            RSAKeyPair user2 = RSAHandler.GenerateKeyPair();


            string myMessage = "I am a nice and friendly text";
            string mySignature = "SignedByMe";

            Console.WriteLine("Send Message from 1 to 2");


            string messageEncrypted = RSAHandler.Encrypt(user2.PublicKey, myMessage);
            string signatureEncrypted = RSAHandler.Sign(user1.PrivateKey, mySignature);

            string messageDecrypted = RSAHandler.Decrypt(user2.PrivateKey, messageEncrypted);
            bool signatureDecrypted = RSAHandler.Verify(user1.PublicKey, mySignature, signatureEncrypted);

            Console.WriteLine("User 2 received message: " + messageDecrypted);
            Console.WriteLine("User 2 received signature: " + signatureDecrypted);

            Console.ReadKey();
        }
    }
}
