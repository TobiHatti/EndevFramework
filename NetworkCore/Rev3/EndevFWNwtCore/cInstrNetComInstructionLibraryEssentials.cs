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
        public class MySampleInstruction : ISB
        {
            public MySampleInstruction(NetComUser pUser, string pValue) : base(pUser, pValue, null) { }

            public override void Execute()
            {
                System.Windows.Forms.MessageBox.Show(value);
            }
        }

        public class MyParamInstruction : ISB
        {
            public MyParamInstruction(NetComUser pUser, string pValue, params object[] pParameters)
                : base(pUser, pValue, pParameters) { }

            public MyParamInstruction(NetComUser pUser, string pValue, string myValue, int myNumber, char myCharacter) 
                : this(pUser, pValue, new object[] { myValue, myNumber, myCharacter }) { }

           
            public override void Execute()
            {
                System.Windows.Forms.MessageBox.Show(value);
            }
        }
    }
}
