using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Segment Complete [Last Modified 31.10.2019]

namespace EndevFWNwtCore
{
    /// <summary>
    /// =====================================   <para />
    /// FRAMEWORK: EndevFrameworkNetworkCore    <para />
    /// SUB-PACKAGE: Instruction-Objects        <para />
    /// =====================================   <para />
    /// DESCRIPTION:                            <para />
    /// Provides the base-foundation for NetCom-
    /// Instructions.
    /// </summary>
    public abstract class InstructionBase
    {
        protected NetComUser sender = null;
        protected NetComUser receiver = null;
        
        protected string instruction = null;
        protected string sInstruction = null;
        protected string value = null;
        protected object[] parameters = null;

        /// <summary>
        /// Version of the currently used Instruction-Set (Instruction-Library)
        /// </summary>
        public static string InstructionSetVersion { get; set; } = "1.0";

        /// <summary>
        /// Current version of the Framework
        /// </summary>
        public static string FrameworkVersion { get; } = "1.1 R3";


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
            receiver = pReceiver;
            sender = pSender;
            value = pValue;
            parameters = pParameters;
            instruction = this.GetType().AssemblyQualifiedName;
            sInstruction = this.GetType().Name;
        }

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
            bool rsaEncryption = false;
            if (receiver.RSAKeys.PublicKey != null) rsaEncryption = true;

            StringBuilder innersb = new StringBuilder();
            StringBuilder sb = new StringBuilder();

            if (rsaEncryption) sb.Append("RSA:");

            // Version Data (Framework and instructionset)
            innersb.Append($"FWV:{Base64Handler.Encode(FrameworkVersion)},");
            innersb.Append($"ISV:{Base64Handler.Encode(InstructionSetVersion)},");

            // User Data
            if (sender?.RSAKeys.PublicKey != null) innersb.Append($"PUK:{Base64Handler.Encode(sender?.RSAKeys.PublicKey)},");
            if (sender?.Username != null) innersb.Append($"USR:{Base64Handler.Encode(sender?.Username)},");
            if (sender?.Password != null)
            {
                if(rsaEncryption) innersb.Append($"PSW:{RSAHandler.Encrypt(receiver?.RSAKeys.PublicKey, sender?.Password)},");
                else innersb.Append($"PSW:{Base64Handler.Encode(sender?.Password)},");
            }
            
            // Signature
            if(sender?.RSAKeys.PrivateKey != null)
            {
                Guid signature = Guid.NewGuid();

                innersb.Append($"SGP:{Base64Handler.Encode(signature.ToString())},");
                innersb.Append($"SGP:{RSAHandler.Sign(sender?.RSAKeys.PrivateKey, signature.ToString())},");
            }

            // Actuall data
            if(instruction != null) innersb.Append($"INS:{Base64Handler.Encode(instruction)},");
            if(value != null)       innersb.Append($"VAL:{Base64Handler.Encode(value)},");

            if (parameters != null)
            {
                innersb.Append($"PAR:");
                foreach (object param in parameters)
                    innersb.Append($"{Base64Handler.Encode(param.GetType().AssemblyQualifiedName)}#{Base64Handler.Encode(param.ToString())}|");
                innersb.Append($",");
            }

            sb.Append(Base64Handler.Encode(innersb.ToString()));
            sb.Append(";");

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

            if (receiver?.RSAKeys.PublicKey != null) isRSAEncrypted = true;
            if (sender?.RSAKeys.PrivateKey != null) isRSASigned = true;

            sb.AppendLine("");
            sb.AppendLine("=================================");
            sb.AppendLine($"Endev NetCore {FrameworkVersion} Instruction");
            sb.AppendLine($"Instruction-Set Version {InstructionSetVersion}");
            sb.AppendLine("=================================");
            sb.AppendLine($"RSA-Encrypted: {isRSAEncrypted}");
            sb.AppendLine($"RSA-Signed: {isRSASigned}");

            if (sender?.Username != null) sb.AppendLine($"Username: {sender?.Username}");
            if (sender?.Password != null) sb.AppendLine($"Password: {sender?.Password}");

            if (instruction != null) sb.AppendLine($"Instruction: {sInstruction}");
            if (value != null) sb.AppendLine($"Value: {value}");

            if (parameters != null)
            {
                sb.AppendLine($"Parameters: ");
                foreach (object param in parameters)
                    sb.AppendLine($" - {param.ToString()} [{param.GetType().Name}]");
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
            if(receiver == null)
            {
                receiver = new NetComUser();
            }

            receiver.SetUserData("", "", pPublicKey);
        }
    }
}
