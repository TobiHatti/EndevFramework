using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NCIB = EndevFWNetCore.NetComInstructionBase;

namespace EndevFWNetCore
{
    public class NetComInstructionLib
    {
        public class PlainText : NCIB
        {
            public override void Execute() =>
                throw new NetComNotImplementedException("*** Instruction [PlainText] has not been implemented yet! ***");
        }
        
        public class MessageBox : NCIB
        {
            public override void Execute() => 
                System.Windows.Forms.MessageBox.Show(
                (string) Value, 
                (string) Parameters[0], 
                (System.Windows.Forms.MessageBoxButtons) Parameters[1], 
                (System.Windows.Forms.MessageBoxIcon) Parameters[2]);
        }

        public class NotifyIcon : NCIB
        {
            public override void Execute() => 
                throw new NetComNotImplementedException("*** Instruction [NotyfiIcon] has not been implemented yet! ***");
        }
    }
}
