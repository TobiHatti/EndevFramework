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
        protected string value = null;
        protected object[] parameters = null;

        public InstructionBase(NetComUser pUser, NetComUser pReceiver, string pValue = null, params object[] pParameters)
        {
            receiver = pReceiver;
            sender = pUser;
            value = pValue;
            parameters = pParameters;
            instruction = this.GetType().AssemblyQualifiedName;   
        }

        /// <summary>
        /// Executes the instruction
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// Encodes the message and returns it as a string
        /// </summary>
        /// <returns>The encoded string</returns>
        public string Encode()
        {
            bool rsaEncryption = false;
            if (receiver.RSAKeys.PublicKey != null) rsaEncryption = true;

            StringBuilder innersb = new StringBuilder();
            StringBuilder sb = new StringBuilder();

            if (rsaEncryption) sb.Append("RSA:");


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

        public sealed override string ToString()
        {
            StringBuilder sb = new StringBuilder();
        }
    }
}
