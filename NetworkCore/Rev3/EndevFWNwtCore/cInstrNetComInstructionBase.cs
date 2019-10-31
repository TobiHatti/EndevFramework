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
    /// SUB-PACKAGE: Instruction-Objects        <para />
    /// =====================================   <para />
    /// DESCRIPTION:                            <para />
    /// Provides the base-foundation for NetCom-
    /// Instructions.
    /// </summary>
    public abstract class InstructionBase
    {
        protected NetComUser user = null;
        protected string instruction = null;
        protected string value = null;
        protected object[] parameters = null;

        public InstructionBase(NetComUser pUser, string pValue = null, params object[] pParameters)
        {
            user = pUser;
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
        public string Encode(NetComUser pReceiver)
        {
            bool rsaEncryption = false;
            if (pReceiver.RSAKeys.PublicKey != null) rsaEncryption = true;

            StringBuilder innersb = new StringBuilder();
            StringBuilder sb = new StringBuilder();

            if (rsaEncryption) sb.Append("RSA:");


            // User Data
            if (user?.RSAKeys.PublicKey != null) innersb.Append($"PUK:{Base64Handler.Encode(user?.RSAKeys.PublicKey)},");
            if (user?.Username != null) innersb.Append($"USR:{Base64Handler.Encode(user?.Username)},");
            if (user?.Password != null)
            {
                if(rsaEncryption) innersb.Append($"PSW:{RSAHandler.Encrypt(pReceiver?.RSAKeys.PublicKey, user?.Password)},");
                else innersb.Append($"PSW:{Base64Handler.Encode(user?.Password)},");
            }
            
            // Signature
            if(user?.RSAKeys.PrivateKey != null)
            {
                Guid signature = Guid.NewGuid();

                innersb.Append($"SGP:{Base64Handler.Encode(signature.ToString())},");
                innersb.Append($"SGP:{RSAHandler.Sign(user?.RSAKeys.PrivateKey, signature.ToString())},");
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
    }
}
