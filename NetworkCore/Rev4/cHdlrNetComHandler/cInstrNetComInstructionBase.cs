using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndevFramework.NetworkCore
{
    public abstract class InstructionBase
    {
        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 1b │ F I E L D S   ( P R O T E C T E D )                    ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝    

        #region ═╣ F I E L D S   ( P R O T E C T E D ) ╠═ 

        protected string instruction = null;
        protected string sInstruction = null;
        protected string value = null;
        protected object[] parameters = null;

        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 2a │ P R O P E R T I E S   ( I N T E R N A L )              ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝  

        #region ═╣ P R O P E R T I E S   ( I N T E R N A L ) ╠═ 

        internal string ID { get; set; } = null;

        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 2b │ P R O P E R T I E S   ( P U B L I C )                  ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝  

        #region ═╣ P R O P E R T I E S   ( P U B L I C ) ╠═ 

        public NetComUser Sender { get; private set; } = null;
        public NetComUser Receiver { get; set; } = null;

        /// <summary>
        /// Version of the currently used Instruction-Set (Instruction-Library)
        /// </summary>
        public static string InstructionSetVersion { get; set; } = "1.2";

        /// <summary>
        /// Current version of the Framework
        /// </summary>
        public static string FrameworkVersion { get; } = "1.5";

        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 3  │ C O N S T R U C T O R S                                ║
        // ╚════╧════════════════════════════════════════════════════════╝  

        #region ═╣ C O N S T R U C T O R S ╠═ 

        /// <summary>
        /// Provides a base for new custom instructions, or 
        /// operates as a common interface for instructions 
        /// </summary>
        /// <param name="pSender">Sender (local user) of the instruction</param>
        /// <param name="pReceiver">Receiver (remote user) of the instruction</param>
        /// <param name="pValue">Value of the instruction. Can be used as required.</param>
        /// <param name="pParameters">Parameters of the instruction. Can be used as required</param>
        public InstructionBase(NetComUser pSender, NetComUser pReceiver, string pValue = null, params object[] pParameters)
        {
            Receiver = pReceiver;
            Sender = pSender;
            value = pValue;
            parameters = pParameters;

            instruction = this.GetType().AssemblyQualifiedName;
            sInstruction = this.GetType().Name;
            ID = Guid.NewGuid().ToString();
        }

        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 4a │ M E T H O D S   ( P R I V A T E )                      ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝  

        #region ═╣ M E T H O D S   ( P R I V A T E ) ╠═ 

        /// <summary>
        /// Encodes a key-exchange-instruction 
        /// from server to client.
        /// </summary>
        /// <returns>The encoded string</returns>
        private string EncodeKeyExchangeS2C()
        {
            StringBuilder innersb = new StringBuilder();
            StringBuilder sb = new StringBuilder();

            innersb.Append($"IID:{ID}$");
            innersb.Append($"FWV:{FrameworkVersion}$");
            innersb.Append($"ISV:{InstructionSetVersion}$");
            innersb.Append($"PUK:{Sender?.RSAKeys.PublicKey}$");
            innersb.Append($"USR:{Base64Handler.Encode("Server")}$");
            innersb.Append($"INS:{instruction}$");

            sb.Append(innersb.ToString());
            sb.Append("%");

            return sb.ToString();
        }

        /// <summary>
        /// Encodes a key-exchange-instruction 
        /// from client to server.
        /// </summary>
        /// <returns>The encoded string</returns>
        private string EncodeKeyExchangeC2S()
        {
            StringBuilder innersb = new StringBuilder();
            StringBuilder sb = new StringBuilder();

            innersb.Append($"IID:{ID}$");
            innersb.Append($"FWV:{FrameworkVersion}$");
            innersb.Append($"ISV:{InstructionSetVersion}$");
            innersb.Append($"PUK:{Sender?.RSAKeys.PublicKey}$");
            innersb.Append($"USR:{Base64Handler.Encode(Sender?.Username)}$");
            innersb.Append($"INS:{instruction}$");

            sb.Append(innersb.ToString());
            sb.Append("%");

            return sb.ToString();
        }

        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 4d │ M E T H O D S   ( P U B L I C )                        ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝ 

        #region ═╣ M E T H O D S   ( P U B L I C ) ╠═ 

        /// <summary>
        /// Executes the instruction.
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// Encodes the message and returns it as a string.
        /// </summary>
        /// <returns>The encoded string</returns>
        public string Encode()
        {

            if (this.GetType() == typeof(InstructionLibraryEssentials.KeyExchangeServer2Client))
            {
                return EncodeKeyExchangeS2C();
            }
            if (this.GetType() == typeof(InstructionLibraryEssentials.KeyExchangeClient2Server))
            {
                return EncodeKeyExchangeC2S();
            }


            bool rsaEncryption = false;
            if (Receiver.RSAKeys.PublicKey != null) rsaEncryption = true;

            StringBuilder innersb = new StringBuilder();
            StringBuilder sb = new StringBuilder();

            if (rsaEncryption) sb.Append("R:");

            // Instruction-ID
            innersb.Append($"IID:{ID}$");

            // Version Data (Framework and instructionset)
            innersb.Append($"FWV:{FrameworkVersion}$");
            innersb.Append($"ISV:{InstructionSetVersion}$");

            // User Data
            if (Sender?.RSAKeys.PublicKey != null) innersb.Append($"PUK:{Sender?.RSAKeys.PublicKey}$");
            if (Sender?.Username != null) innersb.Append($"USR:{Base64Handler.Encode(Sender?.Username)}$");
            if (Sender?.Password != null)
            {
                if (rsaEncryption) innersb.Append($"PSW:{RSAHandler.Encrypt(Receiver?.RSAKeys.PublicKey, Sender?.Password)}$");
                else innersb.Append($"PSW:{Base64Handler.Encode(Sender?.Password)}$");
            }

            // Signature
            if (Sender?.RSAKeys.PrivateKey != null)
            {
                Guid signature = Guid.NewGuid();

                innersb.Append($"SGP:{signature.ToString()}$");
                innersb.Append($"SGC:{RSAHandler.Sign(Sender?.RSAKeys.PrivateKey, signature.ToString())}$");
            }

            // Actuall data
            if (instruction != null) innersb.Append($"INS:{instruction}$");
            if (value != null) innersb.Append($"VAL:{Base64Handler.Encode(value)}$");

            if (parameters != null)
            {
                innersb.Append($"PAR:");
                foreach (object param in parameters)
                    innersb.Append($"{param.GetType().AssemblyQualifiedName}#{Base64Handler.Encode(param.ToString())}|");
                innersb.Append($"$");
            }

            sb.Append(innersb.ToString());
            sb.Append("%");

            return sb.ToString();
        }

        /// <summary>
        /// Returns the instructions data formated.
        /// </summary>
        /// <returns>The formated Instruction-string</returns>
        public sealed override string ToString()
        {
            bool isRSAEncrypted = false;
            bool isRSASigned = false;

            StringBuilder sb = new StringBuilder();

            if (Receiver?.RSAKeys.PublicKey != null) isRSAEncrypted = true;
            if (Sender?.RSAKeys.PrivateKey != null) isRSASigned = true;

            sb.AppendLine("");
            sb.AppendLine("\t=================================");
            sb.AppendLine($"\tEndev NetCore {FrameworkVersion} Instruction");
            sb.AppendLine($"\tInstruction-Set Version {InstructionSetVersion}");
            sb.AppendLine("\t=================================");
            sb.AppendLine($"\tInstruction-ID: {ID}");
            sb.AppendLine($"\tRSA-Encrypted: {isRSAEncrypted}");
            sb.AppendLine($"\tRSA-Signed: {isRSASigned}");

            if (Sender?.Username != null) sb.AppendLine($"\tUsername: {Sender?.Username}");
            if (Sender?.Password != null) sb.AppendLine($"\tPassword: {Sender?.Password}");

            if (instruction != null) sb.AppendLine($"\tInstruction: {sInstruction}");
            if (value != null) sb.AppendLine($"\tValue: {value}");

            if (parameters != null)
            {
                sb.AppendLine($"\tParameters: ");
                foreach (object param in parameters)
                    sb.AppendLine($"\t - {param.ToString()} [{param.GetType().Name}]");
            }

            sb.AppendLine("");

            return sb.ToString();
        }

        /// <summary>
        /// Sets the public-key of the receiver.
        /// </summary>
        /// <param name="pPublicKey">Public-key of the receiver</param>
        public void SetReceiverPublicKey(string pPublicKey)
        {
            if (Receiver == null)
            {
                Receiver = new NetComUser();
            }

            Receiver.SetUserData("", "", pPublicKey);
        }

        /// <summary>
        /// Clones the current instruction.
        /// </summary>
        /// <returns>New instance of the same instruction (clone)</returns>
        public InstructionBase Clone()
        {
            InstructionBase retInstr;
            if (value == null && parameters == null)
                retInstr = (InstructionBase)Activator.CreateInstance(Type.GetType(instruction), Sender, Receiver);
            else if (parameters == null)
                retInstr = (InstructionBase)Activator.CreateInstance(Type.GetType(instruction), Sender, Receiver, value);
            else
                retInstr = (InstructionBase)Activator.CreateInstance(Type.GetType(instruction), Sender, Receiver, value, parameters);

            retInstr.ID = ID;
            retInstr.instruction = instruction;
            retInstr.sInstruction = sInstruction;
            retInstr.Sender = Sender;
            retInstr.Receiver = Receiver;
            retInstr.value = value;
            retInstr.parameters = parameters;

            return retInstr;
        }

        #endregion
    }
}
