using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        protected string username = null;
        protected string password = null;
        protected string instruction = null;
        protected string value = null;
        protected object[] parameters = null;

        public InstructionBase(NetComUser pUser)
        {

        }

        /// <summary>
        /// Executes the instruction
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// Encodes the message and returns it as a string
        /// </summary>
        /// <returns>The encoded string</returns>
        public sealed override string ToString()
            => Encode();

        /// <summary>
        /// Encodes the message and returns it as a string
        /// </summary>
        /// <returns>The encoded string</returns>
        public string Encode()
        {
            return "";
        }
    }
}
