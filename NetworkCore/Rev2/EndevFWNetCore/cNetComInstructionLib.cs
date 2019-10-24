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
            public PreAuth(INetComUser pUser, string pVal, object[] pParam, string pRepReq) 
                : this(pUser) { }

            public PreAuth(INetComUser pUser) : base(pUser)
            {
                MsgType = MessageType.PREAUTH;
                Instruction = this.GetType().Name;

                if (pUser.GetType() == typeof(NetComServer)) Value = (pUser as NetComServer).RSA.PublicKey;
                if (pUser.GetType() == typeof(NetComClient)) Value = (pUser as NetComClient).RSA.PublicKey;
            }

            public override void Execute() =>
                throw new NetComNotImplementedException("*** Instruction [PreAuth] has not been implemented yet! ***");
        }


        //==================================================================================================================

        public class PlainText : NCI
        {
            public PlainText(INetComUser pUser, string pVal, object[] pParam, string pRepReq) 
                : this(pUser, pVal) { }

            public PlainText(INetComUser pUser, string pMessage) 
                : base(pUser, pMessage) => Instruction = this.GetType().Name;

            public override void Execute() =>
                throw new NetComNotImplementedException("*** Instruction [PlainText] has not been implemented yet! ***");
        }

        //==================================================================================================================

        public class MessageBox : NCI
        {
            public MessageBox(INetComUser pUser, string pVal, object[] pParam, string pRepReq) 
                : this(pUser, pVal, (string)pParam[0], (System.Windows.Forms.MessageBoxButtons)pParam[1], (System.Windows.Forms.MessageBoxIcon)pParam[2]) { }

            public MessageBox(INetComUser pUser, string pMessage, string pCaption, System.Windows.Forms.MessageBoxButtons pButtons, System.Windows.Forms.MessageBoxIcon pIcons) 
                : base(pUser, pMessage, new object[] { pCaption, pButtons, pIcons }) => Instruction = this.GetType().Name;

            public override void Execute() => 
                System.Windows.Forms.MessageBox.Show(
                (string) Value, 
                (string) Parameters[0], 
                (System.Windows.Forms.MessageBoxButtons) Parameters[1], 
                (System.Windows.Forms.MessageBoxIcon) Parameters[2]);
        }

        //==================================================================================================================

        public class DecoratedMessageBox : NCI
        {
            public DecoratedMessageBox(INetComUser pUser, string pVal, object[] pParam, string pRepReq) 
                : this(pUser, pVal) { }

            public DecoratedMessageBox(INetComUser pUser, string pMessage)
                : base(pUser, pMessage) => Instruction = this.GetType().Name;

            public override void Execute() =>
                System.Windows.Forms.MessageBox.Show(Value);
        }

        //==================================================================================================================

        public class NotifyIcon : NCI
        {
            public NotifyIcon(INetComUser pUser, string pVal, object[] pParam, string pRepReq)
                : this(pUser, pVal) { }

            public NotifyIcon(INetComUser pUser, string pMessage)
                : base(pUser, pMessage) => Instruction = this.GetType().Name;

            public override void Execute() => 
                throw new NetComNotImplementedException("*** Instruction [NotyfiIcon] has not been implemented yet! ***");
        }

        //==================================================================================================================
    }
}
