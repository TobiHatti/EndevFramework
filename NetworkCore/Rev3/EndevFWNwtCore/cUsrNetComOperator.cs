using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EndevFWNwtCore
{
    /// <summary>
    /// =====================================   <para />
    /// FRAMEWORK: EndevFrameworkNetworkCore    <para />
    /// SUB-PACKAGE: User-Objects               <para />
    /// =====================================   <para />
    /// DESCRIPTION:                            <para />
    /// Basic object for NetCom-Operators 
    /// (Client/Server)
    /// </summary>
    public abstract class NetComOperator : NetComUser
    {
        protected IPAddress serverIP = null;
        protected int port = 2225;

        protected const int bufferSize = 1024;
        protected volatile byte[] buffer = new byte[bufferSize];

        protected volatile List<InstructionBase> incommingInstructions = new List<InstructionBase>();
        protected volatile List<InstructionBase> outgoingInstructions = new List<InstructionBase>();

        protected delegate void DebuggingOutput(string pDebugMessage, params object[] pParameters);
        protected DebuggingOutput DebugCom = null;
        protected object[] debugParams = null;

        protected volatile Thread instructionProcessingThread = null;
        protected volatile Thread instructionSendingThread = null;

        protected volatile int threadIdleTime = 100;

        
        protected void Debug(string pMessage)
        {
            DebugCom(pMessage, debugParams);
        }

        protected void AsyncInstructionSendingLoop()
        {
            while(true)
            {
                if (outgoingInstructions.Count > 0) 
                    AsyncInstructionSendNext();
                Thread.Sleep(threadIdleTime);
            }
        }

        protected void AsyncInstructionProcessingLoop()
        {
            while (true)
            {
                if (incommingInstructions.Count > 0)
                    AsyncInstructionProcessNext();
                Thread.Sleep(threadIdleTime);
            }
        }

        protected abstract void AsyncInstructionSendNext();
        protected abstract void AsyncInstructionProcessNext();

        
    }
}
