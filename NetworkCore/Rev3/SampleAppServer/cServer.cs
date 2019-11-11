using EndevFWNwtCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ILE = EndevFWNwtCore.InstructionLibraryExtension;
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



            NetComServer server = new NetComServer(2225);

            server.SetDebugOutput(DebugOutput.ToConsole);

            server.Start();

            // The server can send to a range of connected clients, wich can be selected in the server.ConnectedClients-Property

            Thread.Sleep(2000);

            while(true)
            {
                server.Send(new ILE.MyStabilityTest(server, server.ConnectedClients[0]));

                //server.Broadcast(new ILE.MySampleInstruction(server, null, "Broadcast"));

                Thread.Sleep(1000);
            }
            

            Console.ReadKey();

            //NetComUser user = new NetComUser(); 
            //NetComUser rec = new NetComUser();
            

            //InstructionBase instr1 = new InstructionLibraryExtension.MySampleInstruction(user, "Hallo");
            //InstructionBase instr2 = new InstructionLibraryExtension.MySampleInstruction(user, "i");
            //InstructionBase instr3 = new InstructionLibraryExtension.MySampleInstruction(user, "bin");
            //InstructionBase instr4 = new InstructionLibraryExtension.MySampleInstruction(user, "da");
            //InstructionBase instrX = new InstructionLibraryExtension.MyParamInstruction(user, "da", "Test", 32, 'L');
            //InstructionBase instr5 = new InstructionLibraryExtension.MySampleInstruction(user, "Adam");
            //InstructionBase instr6 = new InstructionLibraryExtension.MySampleInstruction(user, "aus");
            //InstructionBase instr7 = new InstructionLibraryExtension.MySampleInstruction(user, "Tirol");

            //string msgString = instr1.Encode(rec);
            //msgString += instr2.Encode(rec);
            //msgString += instr3.Encode(rec);
            //msgString += instr4.Encode(rec);
            //msgString += instr5.Encode(rec);
            //msgString += instrX.Encode(rec);
            //msgString += instr6.Encode(rec);
            //msgString += instr7.Encode(rec);

            //Console.WriteLine(msgString);


            //InstructionBase[] result = InstructionOperations.Parse(user, null, msgString).ToArray();

            //foreach (InstructionBase res in result)
            //    res.Execute();


            //RSAKeyPair user1 = RSAHandler.GenerateKeyPair();
            //RSAKeyPair user2 = RSAHandler.GenerateKeyPair();


            //string myMessage = "I am a nice and friendly text";
            //string mySignature = "SignedByMe";

            //Console.WriteLine("Send Message from 1 to 2");


            //string messageEncrypted = RSAHandler.Encrypt(user2.PublicKey, myMessage);
            //string signatureEncrypted = RSAHandler.Sign(user1.PrivateKey, mySignature);

            //string messageDecrypted = RSAHandler.Decrypt(user2.PrivateKey, messageEncrypted);
            //bool signatureDecrypted = RSAHandler.Verify(user1.PublicKey, mySignature, signatureEncrypted);

            //Console.WriteLine("User 2 received message: " + messageDecrypted);
            //Console.WriteLine("User 2 received signature: " + signatureDecrypted);

            //Console.ReadKey();
        }
    }
}
