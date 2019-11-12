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
                (Receiver as NetComOperator).Debug($"SAMPLE-INSTRUCTION RECEIVED!");
            }
        }


        public class __AuthenticationServer2Client : ISB
        {
            public __AuthenticationServer2Client(NetComUser pSender, NetComUser pReceiver, string pValue)
                : base(pSender, pReceiver, pValue, null) { }

            public __AuthenticationServer2Client(NetComUser pSender, NetComUser pReceiver) 
                : base(pSender, pReceiver, pSender.RSAKeys.PublicKey, null) { }

            public override void Execute()
            {
                (Receiver as NetComClient).SetServerRSA(value);
                (Receiver as NetComClient).Send(new InstructionLibraryEssentials.__AuthenticationClient2Server(Receiver, null));
            }
        }

        public class __AuthenticationClient2Server : ISB
        {
            public __AuthenticationClient2Server(NetComUser pSender, NetComUser pReceiver, string pValue, object[] pParameters) 
                : base(pSender, pReceiver, pValue, pParameters) { }
            public __AuthenticationClient2Server(NetComUser pSender, NetComUser pReceiver) 
                : base(pSender, pReceiver, pSender.RSAKeys.PublicKey, new object[] { pSender.Username }) { }

            public override void Execute()
            {
                (Receiver as NetComServer).CurrentProcessingClient.SetUserData(parameters[0].ToString(), "", value);
            }
        }

        public class SimpleMessageBox : ISB
        {
            public SimpleMessageBox(NetComUser pSender, NetComUser pReceiver, string pMessage)
                : base(pSender, pReceiver, pMessage, null) { }


            public override void Execute()
            {
                System.Windows.Forms.MessageBox.Show(value);
            }
        }

        public class RichMessageBox : ISB
        {
            public RichMessageBox(NetComUser pSender, NetComUser pReceiver, string pValue, object[] pParameters)
                : base(pSender, pReceiver, pValue, pParameters) { }

            public RichMessageBox(NetComUser pSender, NetComUser pReceiver, string pMessage, string pCaption, System.Windows.Forms.MessageBoxButtons pButtons, System.Windows.Forms.MessageBoxIcon pIcon)
                : base(pSender, pReceiver, pMessage, null) 
            {
                object[] prm = new object[3];

                prm[0] = pCaption;

                switch(pButtons)
                {
                    case System.Windows.Forms.MessageBoxButtons.AbortRetryIgnore:
                        prm[1] = "MB1";
                        break;
                    case System.Windows.Forms.MessageBoxButtons.OK:
                        prm[1] = "MB2";
                        break;
                    case System.Windows.Forms.MessageBoxButtons.OKCancel:
                        prm[1] = "MB3";
                        break;
                    case System.Windows.Forms.MessageBoxButtons.RetryCancel:
                        prm[1] = "MB4";
                        break;
                    case System.Windows.Forms.MessageBoxButtons.YesNo:
                        prm[1] = "MB5";
                        break;
                    case System.Windows.Forms.MessageBoxButtons.YesNoCancel:
                        prm[1] = "MB6";
                        break;
                }

                switch (pIcon)
                {
                    case System.Windows.Forms.MessageBoxIcon.Information:
                        prm[2] = "MI1";
                        break;
                    case System.Windows.Forms.MessageBoxIcon.Error:
                        prm[2] = "MI2";
                        break;
                    case System.Windows.Forms.MessageBoxIcon.Exclamation:
                        prm[2] = "MI3";
                        break;
                    case System.Windows.Forms.MessageBoxIcon.None:
                        prm[2] = "MI4";
                        break;
                    case System.Windows.Forms.MessageBoxIcon.Question:
                        prm[2] = "MI5";
                        break;
                }

                parameters = prm;
            }

            public override void Execute()
            {
                System.Windows.Forms.MessageBoxButtons button = 0;
                System.Windows.Forms.MessageBoxIcon icon = 0;

                switch (parameters[1])
                {
                    case "MB1":
                        button = System.Windows.Forms.MessageBoxButtons.AbortRetryIgnore;
                        break;
                    case "MB2":
                        button = System.Windows.Forms.MessageBoxButtons.OK;
                        break;
                    case "MB3":
                        button = System.Windows.Forms.MessageBoxButtons.OKCancel;
                        break;
                    case "MB4":
                        button = System.Windows.Forms.MessageBoxButtons.RetryCancel;
                        break;
                    case "MB5":
                        button = System.Windows.Forms.MessageBoxButtons.YesNo;
                        break;
                    case "MB6":
                        button = System.Windows.Forms.MessageBoxButtons.YesNoCancel;
                        break;
                }

                switch (parameters[2])
                {
                    case "MI1":
                        icon = System.Windows.Forms.MessageBoxIcon.Information;
                        break;
                    case "MI2":
                        icon = System.Windows.Forms.MessageBoxIcon.Error;
                        break;
                    case "MI3":
                        icon = System.Windows.Forms.MessageBoxIcon.Exclamation;
                        break;
                    case "MI4":
                        icon = System.Windows.Forms.MessageBoxIcon.None;
                        break;
                    case "MI5":
                        icon = System.Windows.Forms.MessageBoxIcon.Question;
                        break;
                }

                System.Windows.Forms.MessageBox.Show(value, parameters[0].ToString(), button, icon);
            }
        }


    }
}
