using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NCI = EndevFWNetCore.NetComInstruction;

namespace EndevFWNetCore
{
    

    public class NetComInstructionLib
    {
        //==================================================================================================================
        public class PreAuth : NCI
        {
            public PreAuth(INetComUser pUser) : base(pUser)
            {
                MsgType = MessageType.PREAUTH;
                Instruction = Instruction.PREAUTH;

                if (pUser.GetType() == typeof(NetComServer)) Value = (pUser as NetComServer).RSA.PublicKey;
                if (pUser.GetType() == typeof(NetComClient)) Value = (pUser as NetComClient).RSA.PublicKey;
            }

            public override void Execute() =>
                throw new NetComNotImplementedException("*** Instruction [PlainText] has not been implemented yet! ***");
        }


        //==================================================================================================================

        public class PlainText : NCI
        {
            public PlainText(INetComUser pUser, string pMessage) : base(pUser)
            {
                Instruction = Instruction.PLAINTEXT;
                Value = pMessage;
            }

            public override void Execute() =>
                throw new NetComNotImplementedException("*** Instruction [PlainText] has not been implemented yet! ***");
        }

        //==================================================================================================================

        public class MessageBox : NCI
        {
            public MessageBox(INetComUser pUser, string pMessage) : base(pUser)
            {
                Instruction = Instruction.MESSAGEBOX;
                Value = pMessage;
            }

            public MessageBox(INetComUser pUser, string pMessage, string pCaption, System.Windows.Forms.MessageBoxButtons pButtons, System.Windows.Forms.MessageBoxIcon pIcons) : base(pUser)
            {
                Instruction = Instruction.MESSAGEBOX;
                Value = pMessage;
                Parameters = new object[] { pCaption, pButtons, pIcons };
            }

            public override void Execute() => 
                System.Windows.Forms.MessageBox.Show(
                (string) Value, 
                (string) Parameters[0], 
                (System.Windows.Forms.MessageBoxButtons) Parameters[1], 
                (System.Windows.Forms.MessageBoxIcon) Parameters[2]);
        }

        //==================================================================================================================

        public class NotifyIcon : NCI
        {
            public NotifyIcon(INetComUser pUser, string pMessage) : base(pUser)
            {
                Instruction = Instruction.PLAINTEXT;
                Value = pMessage;
            }

            public override void Execute() => 
                throw new NetComNotImplementedException("*** Instruction [NotyfiIcon] has not been implemented yet! ***");
        }

        //==================================================================================================================
    }
}
