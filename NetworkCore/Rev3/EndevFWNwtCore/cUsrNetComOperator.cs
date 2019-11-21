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
        protected ulong _logErrorCount = 0;
        protected ulong _logTotalInstructionCount = 0;

        // Funfact: ulong and 100KB allows 1,8 Yottabytes (1.800.000.000.000.000 GB) 
        //to be sent before an overflow-exception gets thrown.

        public ulong TotalSendCounter { get; internal set; } = 0;
        public ulong TotalReceiveCounter { get; internal set; } = 0;

        public bool AutoRestartOnCrash { get; set; } = true;
        public bool ShowExceptions { get; set; } = true;

        protected volatile bool haltActive = false;

        protected const int bufferSize = 102400; // 100KB (KiB)
        protected volatile byte[] buffer = new byte[bufferSize];

        protected volatile List<InstructionBase> incommingInstructions = new List<InstructionBase>();
        internal volatile List<InstructionBase> OutgoingInstructions = new List<InstructionBase>();

        // Stores the ID's of all incomming instructions
        internal volatile List<string> InstructionLogIncomming = new List<string>();

        // Stores a copy of every instruction that gets sent in case it needs to be re-send
        internal volatile InstructionOutLog InstructionLogOutgoing = new InstructionOutLog();

        public delegate void DebuggingOutput(string pDebugMessage, DebugType pType, params object[] pParameters);
        protected DebuggingOutput DebugCom = null;
        protected object[] debugParams = null;

        protected volatile Thread instructionProcessingThread = null;
        protected volatile Thread instructionSendingThread = null;
        protected volatile Thread longTermOperationThread = null;

        internal List<string> OutputStream { get; private set; } = new List<string>();

        protected volatile int threadIdleTime = 100;
        protected volatile int longTermInstructionSleepInMinutes = 1;

        /// <summary>
        /// Starts all tasks required for the Client and Server
        /// </summary>
        public virtual void Start()
        {
            Debug("Starting Background-Process: Instruction-Processing...", DebugType.Info);
            instructionProcessingThread = new Thread(AsyncInstructionProcessingLoop);
            instructionProcessingThread.Start();

            Debug("Starting Background-Process: Instruction-Sending...", DebugType.Info);
            instructionSendingThread = new Thread(AsyncInstructionSendingLoop);
            instructionSendingThread.Start();

            if (!haltActive)
            {
                Debug("Starting Background-Process: Long-Term-Operations...", DebugType.Info);
                longTermOperationThread = new Thread(AsyncLongTermInstructionLoop);
                longTermOperationThread.Start();
            }

            //if (instructionProcessingThread?.IsAlive == false)
            //{
            //    Debug("Starting Background-Process: Instruction-Processing...", DebugType.Info);
            //    instructionProcessingThread = new Thread(AsyncInstructionProcessingLoop);
            //    instructionProcessingThread.Start();
            //}
            //else Debug("Instruction-Processing is already active.", DebugType.Warning);

            //if (instructionSendingThread?.IsAlive == false)
            //{
            //    Debug("Starting Background-Process: Instruction-Sending...", DebugType.Info);
            //    instructionSendingThread = new Thread(AsyncInstructionSendingLoop);
            //    instructionSendingThread.Start();
            //}
            //else Debug("Instruction-Sending is already active.", DebugType.Warning);

            //if (!haltActive)
            //{
            //    if (instructionSendingThread?.IsAlive == false)
            //    {
            //        Debug("Starting Background-Process: Long-Term-Operations...", DebugType.Info);
            //        longTermOperationThread = new Thread(AsyncLongTermInstructionLoop);
            //        longTermOperationThread.Start();
            //    }
            //    else Debug("Background-Worker is already active.", DebugType.Warning);
            //}
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
        internal void Debug(string pMessage, DebugType pDebugType = DebugType.Info)
        {
            DebugCom(pMessage, pDebugType, debugParams);
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
                    if (OutgoingInstructions.Count > 0)
                        AsyncInstructionSendNext();
                    else
                        Thread.Sleep(threadIdleTime);
                }
            }
            catch (Exception ex)
            {
                Debug("Halting (10)", DebugType.Warning);
                if (ShowExceptions) Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                if (AutoRestartOnCrash) HaltAllThreads();
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
                    if (incommingInstructions.Count > 0)
                        AsyncInstructionProcessNext();
                    else
                        Thread.Sleep(threadIdleTime);
                }
            }
            catch (Exception ex)
            {
                Debug("Halting (09)", DebugType.Warning);
                if (ShowExceptions) Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                if (AutoRestartOnCrash) HaltAllThreads();
            }
        }

        /// <summary>
        /// Loop for executing the AsyncLongTermNextCycle-Method.
        /// </summary>
        protected void AsyncLongTermInstructionLoop()
        {
            Thread.Sleep(10000);
            while (true)
            {
                Debug("Executing Long-Term Operations...", DebugType.Cronjob);
                AsyncLongTermNextCycle();
                Thread.Sleep(longTermInstructionSleepInMinutes * 1000 * 60);
            }
        }

        /// <summary>
        /// Sends the next instruction from the outgoing-queue.
        /// </summary>
        protected virtual void AsyncInstructionSendNext()
        {
            if (!haltActive)
            {
                TotalSendCounter++;
                if(OutgoingInstructions[0].GetType() != typeof(InstructionLibraryEssentials.ReceptionConfirmation))
                    InstructionLogOutgoing.Add(OutgoingInstructions[0].Clone());
            }
        }

        /// <summary>
        /// Processes next instruction in the incomming-queue.
        /// </summary>
        protected virtual void AsyncInstructionProcessNext()
        {
            incommingInstructions[0].Execute();
            incommingInstructions.RemoveAt(0);
            _logTotalInstructionCount++;
            Debug($"Processed Instruction ({_logTotalInstructionCount} - Success-Rate: {(float)(1-((float)InstructionLogOutgoing.Count / (float)_logTotalInstructionCount)) * 100}%, {InstructionLogOutgoing.Count} Missing confirmation)", DebugType.Info);
        }

        /// <summary>
        /// Executes tasks every few minutes. Used for cleanup, improvements, etc.
        /// </summary>
        protected virtual void AsyncLongTermNextCycle()
        {
            CheckHaltingState();
            CheckInstructionIntegrity();
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

        protected void CheckHaltingState()
        {
            Debug("Checking halting-state...", DebugType.Cronjob);
            if (haltActive)
            {
                Debug("Halting active! Restarting system...", DebugType.Cronjob);
                haltActive = false;
                RestartSystem();
            }
            else Debug("Halting disabled. Continuing regular operation.", DebugType.Cronjob);
        }

        protected void CheckInstructionIntegrity()
        {
            Debug("Checking instruction-integrity...", DebugType.Cronjob);

            int msgCtr = 0;

            // Remove instruction that failed to be resent 5 times
            for (int i = InstructionLogOutgoing.Count - 1; i >= 0; i--)
                if (InstructionLogOutgoing.GetAttempts(i) > 20)
                    InstructionLogOutgoing.RemoveAt(i);

            // Re-Send every instruction that is still in the outgoing-log
            for (int i = 0; i < InstructionLogOutgoing.Count; i++)
            {
                OutgoingInstructions.Add(InstructionLogOutgoing[i].Clone());
                InstructionLogOutgoing.AddAttempt(i);
                msgCtr++;
            }

            Debug($"Integrity-Check done. Re-Sent {msgCtr} message(s)", DebugType.Cronjob);
        }

        /// <summary>
        /// Halts all threads and prepares for a restart.
        /// </summary>
        protected virtual void HaltAllThreads()
        {
            if (!haltActive)
            {
                haltActive = true;

                Debug("A fatal error occured. Attempting to halt all processes...", DebugType.Fatal);

                // Terminate threads
                Debug("Halting Instruction-Processing...", DebugType.Fatal);
                try
                {
                    instructionProcessingThread.Abort();
                    Debug("Successfully stopped Instruction-Processing!", DebugType.Fatal);
                }
                catch (Exception ex)
                { 
                    Debug("Could not stop Instruction-Processing!", DebugType.Fatal);
                    if (ShowExceptions) Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                }

                Debug("Halting Instruction-Sending...", DebugType.Fatal);
                try
                {
                    instructionSendingThread.Abort();
                    Debug("Successfully stopped Instruction-Sending!", DebugType.Fatal);
                }
                catch (Exception ex)
                { 
                    Debug("Could not stop Instruction-Sending!", DebugType.Fatal);
                    if (ShowExceptions) Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                }
            }
        }

        /// <summary>
        /// Confirms that the instruction reached the receiver and therefor 
        /// can be removed from the outgoing-log
        /// </summary>
        /// <param name="pInstructionID">ID of the Instruction</param>
        internal void ConfirmExecution(string pInstructionID)
        {
            for (int i = 0; i < InstructionLogOutgoing.Count; i++)
                if (InstructionLogOutgoing[i].ID == pInstructionID)
                {
                    InstructionLogOutgoing.RemoveAt(i);
                }      
        }

        /// <summary>
        /// Restarts the system.
        /// </summary>
        protected virtual void RestartSystem()
        {
            // Re-Define arrays and lists
            buffer = new byte[bufferSize];
            incommingInstructions = new List<InstructionBase>();
            OutgoingInstructions = new List<InstructionBase>();
        }
    }
}
