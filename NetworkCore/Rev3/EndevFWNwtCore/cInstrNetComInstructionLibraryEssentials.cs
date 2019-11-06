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
        public class MyStabilityTest : ISB
        {
            public MyStabilityTest(NetComUser pSender, NetComUser pReceiver, string pValue) : base(pSender, pReceiver, pValue, null) { }

            public override void Execute()
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"INTEGRITY OK!");
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
            }
        }

        public class MySampleInstruction : ISB
        {
            public MySampleInstruction(NetComUser pSender, NetComUser pReceiver, string pValue) : base(pSender, pReceiver, pValue, null) { }

            public override void Execute()
            {
                System.Windows.Forms.MessageBox.Show(value);
            }
        }

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
