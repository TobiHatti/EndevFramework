using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ISB = EndevFrameworkNetworkCore.InstructionBase;

namespace EndevFrameworkNetworkCore
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
        /// [SERVER ONLY]
        /// For Internal use only.
        /// Sends the server's public key to the client.
        /// Required for RSA-Authentication.
        /// </summary>
        internal class KeyExchangeServer2Client : ISB
        {
            public KeyExchangeServer2Client(NetComUser pSender, NetComUser pReceiver, string pValue)
                : base(pSender, pReceiver, pValue, null) { }

            public KeyExchangeServer2Client(NetComUser pSender, NetComUser pReceiver) 
                : base(pSender, pReceiver, pSender.RSAKeys.PublicKey, null) { }

            public override void Execute()
            {
                (Receiver as NetComClient).SetServerRSA(value);
                (Receiver as NetComClient).Send(new InstructionLibraryEssentials.KeyExchangeClient2Server(Receiver, null));
            }
        }

        /// <summary>
        /// [CLIENT ONLY]
        /// For Internal use only.
        /// Sends the client's public key to the server.
        /// Required for RSA-Authentication.
        /// </summary>
        internal class KeyExchangeClient2Server : ISB
        {
            public KeyExchangeClient2Server(NetComUser pSender, NetComUser pReceiver, string pValue, object[] pParameters) 
                : base(pSender, pReceiver, pValue, pParameters) { }
            public KeyExchangeClient2Server(NetComUser pSender, NetComUser pReceiver) 
                : base(pSender, pReceiver, pSender.RSAKeys.PublicKey, new object[] { pSender.Username }) { }

            public override void Execute() 
                => (Receiver as NetComServer).CurrentProcessingClient.SetUserData(parameters[0].ToString(), "", value);
        }

        /// <summary>
        /// [SERVER ONLY]
        /// For Internal use only.
        /// Provides the initial instruction to 
        /// authenticate the server on the client-side.
        /// </summary>
        internal class AuthenticationServer2Client : ISB
        {
            public AuthenticationServer2Client(NetComUser pSender, NetComUser pReceiver)
               : base(pSender, pReceiver, null, null) { }

            public override void Execute()  
                => (Receiver as NetComClient).Send(new InstructionLibraryEssentials.AuthenticationClient2Server(Receiver, null));
        }

        /// <summary>
        /// [CLIENT ONLY]
        /// For Internal use only.
        /// Provides the initial instruction to 
        /// authenticate the client on the server-side.
        /// </summary>
        internal class AuthenticationClient2Server : ISB
        {
            public AuthenticationClient2Server(NetComUser pSender, NetComUser pReceiver)
               : base(pSender, pReceiver, null, null) { }

            public override void Execute() { }
        }

        /// <summary>
        /// [CLIENT / SERVER]
        /// Basic Test-Instruction to check if 
        /// instruction reach the receiver.
        /// </summary>
        public class TestSample : ISB
        {
            public TestSample(NetComUser pSender, NetComUser pReceiver) : base(pSender, pReceiver, null, null) { }

            public override void Execute()
            {
                (Receiver as NetComOperator).Debug($"SAMPLE-INSTRUCTION RECEIVED!");
            }
        }

        /// <summary>
        /// [CLIENT / SERVER]
        /// Writes a message directly to the console.
        /// </summary>
        public class ToConsole : ISB
        {
            public ToConsole(NetComUser pSender, NetComUser pReceiver, string pValue) 
                : base(pSender, pReceiver, pValue, null) { }

            public override void Execute()
            {
                Console.WriteLine(value);
            }
        }

        /// <summary>
        /// [CLIENT / SERVER]
        /// Writes a message to the Debug console window 
        /// (System.Diagnostics.Debug).
        /// </summary>
        public class ToDebug : ISB
        {
            public ToDebug(NetComUser pSender, NetComUser pReceiver, string pValue) 
                : base(pSender, pReceiver, pValue, null) { }

            public override void Execute()
            {
                Debug.Print(value);
            }
        }

        /// <summary>
        /// [CLIENT / SERVER]
        /// Writes a message to the NetComUser's selected Debug-Output
        /// (NetComOperator.Debug).
        /// </summary>
        public class ToNetComDebug : ISB
        {
            public ToNetComDebug(NetComUser pSender, NetComUser pReceiver, string pValue) 
                : base(pSender, pReceiver, pValue, null) { }

            public override void Execute()
            {
                (Receiver as NetComOperator).Debug(value);
            }
        }

        /// <summary>
        /// [CLIENT / SERVER]
        /// Writes a message directly to the console.
        /// Formats the text with a custom foreground- 
        /// and background-color.
        /// </summary>
        public class ToConsoleColored : ISB
        {
            public ToConsoleColored(NetComUser pSender, NetComUser pReceiver, string pValue, object[] pParameters)
                : base(pSender, pReceiver, pValue, pParameters) { }

            public ToConsoleColored(NetComUser pSender, NetComUser pReceiver, string pValue, ConsoleColor pForegroundColor, ConsoleColor pBackgroundColor) 
                : base(pSender, pReceiver, pValue, null) 
            {
                object[] prm = new object[2];

                switch(pForegroundColor)
                {
                    case ConsoleColor.Black: prm[0] = "FG0"; break;
                    case ConsoleColor.Blue: prm[0] = "FG1"; break;
                    case ConsoleColor.Cyan: prm[0] = "FG2"; break;
                    case ConsoleColor.DarkBlue: prm[0] = "FG3"; break;
                    case ConsoleColor.DarkCyan: prm[0] = "FG4"; break;
                    case ConsoleColor.DarkGray: prm[0] = "FG5"; break;
                    case ConsoleColor.DarkGreen: prm[0] = "FG6"; break;
                    case ConsoleColor.DarkMagenta: prm[0] = "FG7"; break;
                    case ConsoleColor.DarkRed: prm[0] = "FG8"; break;
                    case ConsoleColor.DarkYellow: prm[0] = "FG9"; break;
                    case ConsoleColor.Gray: prm[0] = "FGA"; break;
                    case ConsoleColor.Green: prm[0] = "FGB"; break;
                    case ConsoleColor.Magenta: prm[0] = "FGC"; break;
                    case ConsoleColor.Red: prm[0] = "FGD"; break;
                    case ConsoleColor.White: prm[0] = "FGE"; break;
                    case ConsoleColor.Yellow: prm[0] = "FGF"; break;
                }

                switch (pBackgroundColor)
                {
                    case ConsoleColor.Black: prm[1] = "BG0"; break;
                    case ConsoleColor.Blue: prm[1] = "BG1"; break;
                    case ConsoleColor.Cyan: prm[1] = "BG2"; break;
                    case ConsoleColor.DarkBlue: prm[1] = "BG3"; break;
                    case ConsoleColor.DarkCyan: prm[1] = "BG4"; break;
                    case ConsoleColor.DarkGray: prm[1] = "BG5"; break;
                    case ConsoleColor.DarkGreen: prm[1] = "BG6"; break;
                    case ConsoleColor.DarkMagenta: prm[1] = "BG7"; break;
                    case ConsoleColor.DarkRed: prm[1] = "BG8"; break;
                    case ConsoleColor.DarkYellow: prm[1] = "BG9"; break;
                    case ConsoleColor.Gray: prm[1] = "BGA"; break;
                    case ConsoleColor.Green: prm[1] = "BGB"; break;
                    case ConsoleColor.Magenta: prm[1] = "BGC"; break;
                    case ConsoleColor.Red: prm[1] = "BGD"; break;
                    case ConsoleColor.White: prm[1] = "BGE"; break;
                    case ConsoleColor.Yellow: prm[1] = "BGF"; break;
                }

                parameters = prm;
            }

            public override void Execute()
            {
                ConsoleColor oldForeground = Console.ForegroundColor;
                ConsoleColor oldBackground = Console.BackgroundColor;
                ConsoleColor newForground = ConsoleColor.White;
                ConsoleColor newBackground = ConsoleColor.Black;

                switch(parameters[0])
                {
                    case "FG0": newForground = ConsoleColor.Black; break;
                    case "FG1": newForground = ConsoleColor.Blue;  break;
                    case "FG2": newForground = ConsoleColor.Cyan; break;
                    case "FG3": newForground = ConsoleColor.DarkBlue; break;
                    case "FG4": newForground = ConsoleColor.DarkCyan; break;
                    case "FG5": newForground = ConsoleColor.DarkGray; break;
                    case "FG6": newForground = ConsoleColor.DarkGreen; break;
                    case "FG7": newForground = ConsoleColor.DarkMagenta; break;
                    case "FG8": newForground = ConsoleColor.DarkRed; break;
                    case "FG9": newForground = ConsoleColor.DarkYellow; break;
                    case "FGA": newForground = ConsoleColor.Gray; break;
                    case "FGB": newForground = ConsoleColor.Green; break;
                    case "FGC": newForground = ConsoleColor.Magenta; break;
                    case "FGD": newForground = ConsoleColor.Red; break;
                    case "FGE": newForground = ConsoleColor.White; break;
                    case "FGF": newForground = ConsoleColor.Yellow; break;
                }

                switch (parameters[1])
                {
                    case "BG0": newBackground = ConsoleColor.Black; break;
                    case "BG1": newBackground = ConsoleColor.Blue; break;
                    case "BG2": newBackground = ConsoleColor.Cyan; break;
                    case "BG3": newBackground = ConsoleColor.DarkBlue; break;
                    case "BG4": newBackground = ConsoleColor.DarkCyan; break;
                    case "BG5": newBackground = ConsoleColor.DarkGray; break;
                    case "BG6": newBackground = ConsoleColor.DarkGreen; break;
                    case "BG7": newBackground = ConsoleColor.DarkMagenta; break;
                    case "BG8": newBackground = ConsoleColor.DarkRed; break;
                    case "BG9": newBackground = ConsoleColor.DarkYellow; break;
                    case "BGA": newBackground = ConsoleColor.Gray; break;
                    case "BGB": newBackground = ConsoleColor.Green; break;
                    case "BGC": newBackground = ConsoleColor.Magenta; break;
                    case "BGD": newBackground = ConsoleColor.Red; break;
                    case "BGE": newBackground = ConsoleColor.White; break;
                    case "BGF": newBackground = ConsoleColor.Yellow; break;
                }

                Console.ForegroundColor = newForground;
                Console.BackgroundColor = newBackground;
                Console.WriteLine(value);
                Console.ForegroundColor = oldForeground;
                Console.BackgroundColor = oldBackground;
            }
        }

        /// <summary>
        /// [CLIENT / SERVER]
        /// Sends a message to the output-stream 
        /// of the receiver.
        /// </summary>
        public class ToOutputStream : ISB
        {
            public ToOutputStream(NetComUser pSender, NetComUser pReceiver, string pMessage)
                : base(pSender, pReceiver, pMessage, null) { }

            public override void Execute()
            {
                (Receiver as NetComOperator).OutputStream.Add(value);
            }
        }

        /// <summary>
        /// [CLIENT / SERVER]
        /// Shows a simple messagebox to the receiver.
        /// </summary>
        public class SimpleMessageBox : ISB
        {
            public SimpleMessageBox(NetComUser pSender, NetComUser pReceiver, string pMessage)
                : base(pSender, pReceiver, pMessage, null) { }


            public override void Execute()
            {
                MessageBox.Show(value);
            }
        }

        /// <summary>
        /// [CLIENT / SERVER]
        /// Shows a formated messagebox to the receiver.
        /// </summary>
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

        /// <summary>
        /// [CLIENT / SERVER]
        /// Shows a notification-bubble on the 
        /// receiver's screen.
        /// </summary>
        public class NofityIcon : ISB
        {
            public NofityIcon(NetComUser pSender, NetComUser pReceiver, string pValue, object[] pParameters)
                   : base(pSender, pReceiver, pValue, pParameters) { }

            public NofityIcon(NetComUser pSender, NetComUser pReceiver, string pText, string pTitle, int pDisplayDuration, ToolTipIcon pIcon)
                : base(pSender, pReceiver, pText, null)
            {
                object[] prm = new object[3];

                prm[0] = pTitle;
                prm[1] = pDisplayDuration;

                switch(pIcon)
                {
                    case ToolTipIcon.Error: prm[2] = "NI1"; break;
                    case ToolTipIcon.Info: prm[2] = "NI2"; break;
                    case ToolTipIcon.None: prm[2] = "NI3"; break;
                    case ToolTipIcon.Warning: prm[2] = "NI4"; break;
                }

                parameters = prm;
            }

            public override void Execute()
            {
                ToolTipIcon tooltip = ToolTipIcon.None;

                switch (parameters[2].ToString())
                {
                    case "NI1": tooltip = ToolTipIcon.Error; break;
                    case "NI2": tooltip = ToolTipIcon.Info; break;
                    case "NI3": tooltip = ToolTipIcon.None; break;
                    case "NI4": tooltip = ToolTipIcon.Warning; break;
                }

                using (NotifyIcon ballon = new NotifyIcon
                {
                    Visible = true,
                    Icon = SystemIcons.Application
                })
                {
                    ballon.ShowBalloonTip(int.Parse(parameters[1].ToString()), parameters[0].ToString(), value, tooltip);
                }
            }
        }

        /// <summary>
        /// [CLIENT ONLY]
        /// Sends a message from one client to 
        /// another clients output-stream.
        /// </summary>
        public class Message : ISB
        {
            public Message(NetComUser pSender, NetComUser pReceiver, string pValue, object[] pParameters)
                : base(pSender, pReceiver, pValue, pParameters) { }

            public Message(NetComUser pSender, string pReceiverUsername, string pMessage)
                : base(pSender, null, pReceiverUsername, new object[] { pMessage }) { }

            public override void Execute()
            {
                (Receiver as NetComServer).Send(new InstructionLibraryEssentials.ToOutputStream(Receiver, (Receiver as NetComServer).ConnectedClients[value], parameters[0].ToString()));
            }
        }
    }
}
