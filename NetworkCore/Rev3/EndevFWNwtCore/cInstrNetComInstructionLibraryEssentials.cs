using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISB = EndevFWNwtCore.InstructionBase;

namespace EndevFWNwtCore
{
    /// <summary>
    /// =====================================   <para />
    /// FRAMEWORK: EndevFrameworkNetworkCore    <para />
    /// SUB-PACKAGE: Instruction-Objects        <para />
    /// =====================================   <para />
    /// DESCRIPTION:                            <para />
    /// Contains basic use instructions.        <para />
    /// Instructions must inherit ISB 
    /// (InstructionBase) class
    /// </summary>
    public class InstructionLibraryEssentials
    {
        /// <summary>
        /// Basic Test-Instruction to check if 
        /// instruction reach the receiver
        /// </summary>
        public class MyStabilityTest : ISB
        {
            public MyStabilityTest(NetComUser pSender, NetComUser pReceiver) : base(pSender, pReceiver, null, null) { }

            public override void Execute()
            {
                Console.WriteLine($"INSTRUCTION RECEIVED!");
            }
        }

        /// <summary>
        /// Basic Test-Instruction to create a basic 
        /// messagebox with a custom message
        /// </summary>
        public class MySampleInstruction : ISB
        {
            public MySampleInstruction(NetComUser pSender, NetComUser pReceiver, string pMessage) : base(pSender, pReceiver, pMessage, null) { }

            public override void Execute()
            {
                System.Windows.Forms.MessageBox.Show(value);
            }
        }

        /// <summary>
        /// Basic Test-Instruction to check if parameters
        /// are processed correctly
        /// </summary>
        public class MyParamInstruction : ISB
        {
            public MyParamInstruction(NetComUser pSender, NetComUser pReceiver, string pValue, params object[] pParameters)
                : base(pSender, pReceiver, pValue, pParameters) { }

            public MyParamInstruction(NetComUser pSender, NetComUser pReceiver, string pValue, string myValue, int myNumber, char myCharacter) 
                : this(pSender, pReceiver, pValue, new object[] { myValue, myNumber, myCharacter }) { }

           
            public override void Execute()
            {
                System.Windows.Forms.MessageBox.Show(value);
            }
        }
    }
}
