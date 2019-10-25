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
            private NetComUser user;
            public PreAuth(NetComUser pUser, string pVal, object[] pParam, string pRepReq) 
                : this(pUser) { }

            public PreAuth(NetComUser pUser) : base(pUser)
            {
                MsgType = MessageType.PREAUTH;
                Instruction = this.GetType().AssemblyQualifiedName;

                if (pUser.GetType() == typeof(NetComServer)) Value = (pUser as NetComServer).RSA.PublicKey;
                if (pUser.GetType() == typeof(NetComClient)) Value = (pUser as NetComClient).RSA.PublicKey;

                user = pUser;
            }

            public override void Execute()
            {
                if(NetComUser.LocalUser.GetType() == typeof(NetComServer))
                {
                   // (NetComUser.LocalUser as NetComServer).LClientList[(user as NetComUserDummy).Username].PublicKey = (user as NetComUserDummy).PublicKey;
                }

                if (NetComUser.LocalUser.GetType() == typeof(NetComClient))
                {
                    (NetComUser.LocalUser as NetComClient).ServerPublicKey = (user as NetComUserDummy).PublicKey;
                }
                Console.WriteLine("\r\n\r\nReceived Public-Key from Partner: \r\n\r\n" + (user as NetComUserDummy).PublicKey + "\r\n\r\n");
            }
        }


        //==================================================================================================================

        public class PlainText : NCI
        {
            public PlainText(NetComUser pUser, string pVal, object[] pParam, string pRepReq) 
                : this(pUser, pVal) { }

            public PlainText(NetComUser pUser, string pMessage) 
                : base(pUser, pMessage) =>
                Instruction = this.GetType().AssemblyQualifiedName;

            public override void Execute() =>
                Console.WriteLine("\r\n\r\n" + Value + "\r\n\r\n");
        }

        //==================================================================================================================

        public class MessageBox : NCI
        {
            public MessageBox(NetComUser pUser, string pVal, object[] pParam, string pRepReq) 
                : this(pUser, pVal, (string)pParam[0], (System.Windows.Forms.MessageBoxButtons)pParam[1], (System.Windows.Forms.MessageBoxIcon)pParam[2]) { }

            public MessageBox(NetComUser pUser, string pMessage, string pCaption, System.Windows.Forms.MessageBoxButtons pButtons, System.Windows.Forms.MessageBoxIcon pIcons) 
                : base(pUser, pMessage, new object[] { pCaption, pButtons, pIcons }) => Instruction = this.GetType().AssemblyQualifiedName;

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
            public DecoratedMessageBox(NetComUser pUser, string pVal, object[] pParam, string pRepReq) 
                : this(pUser, pVal) { }

            public DecoratedMessageBox(NetComUser pUser, string pMessage)
                : base(pUser, pMessage) => Instruction = this.GetType().AssemblyQualifiedName;

            public override void Execute() =>
                System.Windows.Forms.MessageBox.Show(Value);
        }

        //==================================================================================================================

        public class NotifyIcon : NCI
        {
            public NotifyIcon(NetComUser pUser, string pVal, object[] pParam, string pRepReq)
                : this(pUser, pVal) { }

            public NotifyIcon(NetComUser pUser, string pMessage)
                : base(pUser, pMessage) => Instruction = this.GetType().AssemblyQualifiedName;

            public override void Execute() => 
                throw new NetComNotImplementedException("*** Instruction [NotyfiIcon] has not been implemented yet! ***");
        }

        //==================================================================================================================
    }
}
