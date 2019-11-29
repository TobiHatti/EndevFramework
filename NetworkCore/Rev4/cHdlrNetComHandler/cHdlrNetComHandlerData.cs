using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EndevFrameworkNetworkCore
{
    public class HandlerData
    {
        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 1a │ F I E L D S   ( P R I V A T E )                        ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝    

        #region ═╣ F I E L D S   ( P R I V A T E ) ╠═ 

        private volatile IPAddress serverIP = IPAddress.Loopback;
        private volatile int port = 2225;
        private RSAKeyPair rsaKeys = RSAHandler.GenerateKeyPair();

        private volatile string username = null;
        private volatile string password = null;

        private volatile uint logErrorCounter = 0;
        private volatile uint logProcessCounter = 0;
        private volatile uint logSendCounter = 0;
        private volatile uint logReceiveCounter = 0;

        private volatile bool haltActive = false;
        private volatile bool tryRestartOnCrash = true;
        private volatile bool showExceptions = true;

        private readonly List<InstructionBase> incommingInstructions = new List<InstructionBase>();
        private readonly List<InstructionBase> outgoingInstructions = new List<InstructionBase>();
        private readonly List<string> logIncommingInstructions = new List<string>();
        private readonly InstructionOutgoingLog logOutgoingInstructions = new InstructionOutgoingLog();

        private readonly List<string> outputStream = new List<string>();

        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 2a │ P R O P E R T I E S   ( I N T E R N A L )              ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝  

        #region ═╣ P R O P E R T I E S   ( I N T E R N A L ) ╠═ 

        internal IPAddress ServerIP { get => serverIP; set => serverIP = value; }
        internal int Port { get => port; set => port = value; } 
        internal RSAKeyPair RSAKeys { get => rsaKeys; set => rsaKeys = value; }

        internal string Username { get => username; set => username = value; }
        internal string Password { get => password; set => password = value; }

        internal List<InstructionBase> IncommingInstructions { get => incommingInstructions; }
        internal List<InstructionBase> OutgoingInstructions { get => outgoingInstructions; } 
        internal List<string> LogIncommingInstructions { get => logIncommingInstructions; }
        internal InstructionOutgoingLog LogOutgoingInstructions { get => logOutgoingInstructions; }

        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 2b │ P R O P E R T I E S   ( P U B L I C )                  ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝  

        #region ═╣ P R O P E R T I E S   ( P U B L I C ) ╠═ 

        public uint LogErrorCounter { get => logErrorCounter; internal set => logErrorCounter = value; }
        public uint LogProcessCounter { get => logProcessCounter; internal set => logProcessCounter = value; }
        public uint LogSendCounter { get => logSendCounter; internal set => logSendCounter = value; }
        public uint LogReceiveCounter { get => logReceiveCounter; internal set => logReceiveCounter = value; }

        internal bool HaltActive { get => haltActive; set => haltActive = value; }
        public bool TryRestartOnCrash { get => tryRestartOnCrash; set => tryRestartOnCrash = value; }
        public bool ShowExceptions { get  => showExceptions; set => showExceptions = value; } 

        public List<string> OutputStream { get => outputStream; }

        #endregion
    }
}
