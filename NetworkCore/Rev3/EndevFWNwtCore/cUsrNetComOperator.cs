using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EndevFrameworkNetworkCore
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
        protected int errorCtr = 0;
        protected int processedCount = 0;

        protected bool AutoRestartOnCrash { get; set; } = true;

        protected const int bufferSize = 102400; // 100KB (KiB)
        protected volatile byte[] buffer = new byte[bufferSize];

        protected volatile List<InstructionBase> incommingInstructions = new List<InstructionBase>();
        protected volatile List<InstructionBase> outgoingInstructions = new List<InstructionBase>();

        public delegate void DebuggingOutput(string pDebugMessage, params object[] pParameters);
        protected DebuggingOutput DebugCom = null;
        protected object[] debugParams = null;

        protected volatile Thread instructionProcessingThread = null;
        protected volatile Thread instructionSendingThread = null;
        protected volatile Thread longTermOperationThread = null;

        internal List<string> OutputStream { get; private set; } = new List<string>();

        protected volatile int threadIdleTime = 100;

        protected volatile int longTermInstructionSleepInMinutes = 5;

        public static int queue = 0;

        /// <summary>
        /// Starts all tasks required for the Client and Server
        /// </summary>
        public virtual void Start()
        {
            Debug("Starting Background-Process: Instruction-Processing...");
            instructionProcessingThread = new Thread(AsyncInstructionProcessingLoop);
            instructionProcessingThread.Start();

            Debug("Starting Background-Process: Instruction-Sending...");
            instructionSendingThread = new Thread(AsyncInstructionSendingLoop);
            instructionSendingThread.Start();

            Debug("Starting Background-Process: Long-Term-Operations...");
            longTermOperationThread = new Thread(AsyncLongTermInstructionLoop);
            longTermOperationThread.Start();
        }

        /// <summary>
        /// Sets the debug-output.
        /// Pre-defined debug-outputs can be found in the DebugOutput-Class.
        /// </summary>
        /// <param name="pOutput">Delegate for the debug-output</param>
        /// <param name="pDebugParameters">Optional parameters. See pOutput-Method for more info</param>
        public void SetDebugOutput(DebuggingOutput pOutput, params object[] pDebugParameters)
        {
            DebugCom = pOutput;
            debugParams = pDebugParameters;
        }

        /// <summary>
        /// Sends a debug-message to the selected debug-output.
        /// </summary>
        /// <param name="pMessage">Debug-Message</param>
        internal void Debug(string pMessage)
        {
            DebugCom(pMessage, debugParams);
        }

        /// <summary>
        /// Loop for executing the AsyncInstructionSendNext-Method.
        /// </summary>
        protected void AsyncInstructionSendingLoop()
        {
            try
            {
                while (true)
                {
                    if (outgoingInstructions.Count > 0)
                        AsyncInstructionSendNext();
                    else
                        Thread.Sleep(threadIdleTime);
                }
            }
            catch
            {
                if (AutoRestartOnCrash) RestartSystem();
            }
        }

        /// <summary>
        /// Loop for executing the AsyncInstructionProcessNext-Method.
        /// </summary>
        protected void AsyncInstructionProcessingLoop()
        {
            try
            {
                while (true)
                {
                    queue = incommingInstructions.Count;

                    if (incommingInstructions.Count > 0)
                        AsyncInstructionProcessNext();
                    else
                        Thread.Sleep(threadIdleTime);
                }
            }
            catch
            {
                if (AutoRestartOnCrash) RestartSystem();
            }
        }

        /// <summary>
        /// Loop for executing the AsyncLongTermNextCycle-Method.
        /// </summary>
        protected void AsyncLongTermInstructionLoop()
        {
            try
            {
                while (true)
                {
                    AsyncLongTermNextCycle();
                    Thread.Sleep(longTermInstructionSleepInMinutes * 300000);
                    //Thread.Sleep(5000);
                }
            }
            catch
            {
                if (AutoRestartOnCrash) RestartSystem();
            }
        }

        /// <summary>
        /// Sends the next instruction from the outgoing-queue.
        /// </summary>
        protected abstract void AsyncInstructionSendNext();

        /// <summary>
        /// Processes next instruction in the incomming-queue.
        /// </summary>
        protected virtual void AsyncInstructionProcessNext()
        {
            incommingInstructions[0].Execute();
            incommingInstructions.RemoveAt(0);
            processedCount++;
            Debug($"Processed Instruction ({processedCount} - Success-Rate: {(float)(1-((float)errorCtr / (float)processedCount)) * 100}%)");
        }

        /// <summary>
        /// Executes tasks every few minutes. Used for cleanup, improvements, etc.
        /// </summary>
        protected virtual void AsyncLongTermNextCycle()
        {

        }

        /// <summary>
        /// Gets the next item in the outputstream-queue.
        /// Returns null if queue is empty.
        /// </summary>
        /// <returns>First element of the queue</returns>
        public string ReadOutputStream()
        {
            if (OutputStream.Count == 0) return null;

            string retval = OutputStream[0];
            OutputStream.RemoveAt(0);
            return retval;
        }

        protected virtual void RestartSystem()
        {
            // Terminate threads
            try { instructionProcessingThread.Abort(); } catch { }
            try { instructionSendingThread.Abort(); } catch { }
            try { longTermOperationThread.Abort(); } catch { }

            Debug("Waiting 5 seconds before restart-attempt...");
            Thread.Sleep(5000);

            // Re-Define arrays and lists
            buffer = new byte[bufferSize];
            incommingInstructions = new List<InstructionBase>();
            outgoingInstructions = new List<InstructionBase>();
            OutputStream = new List<string>();
        }
    }
}
