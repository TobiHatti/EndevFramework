using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EndevFramework.NetworkCore
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
        internal NetComHandler Handler { get; set; } = null;
        internal HandlerData HandlerData { get; set; } = null;

        protected volatile Thread instructionProcessingThread = null;
        protected volatile Thread instructionSendingThread = null;

        protected volatile bool haltActive = false;

        protected const int bufferSize = 102400; // 100KB (KiB)
        protected volatile byte[] buffer = new byte[bufferSize];

        protected volatile int threadIdleTime = 100;


        public NetComOperator(NetComHandler pHandler, HandlerData pHandlerData)
        {
            Handler = pHandler;
            HandlerData = pHandlerData;
        }

        /// <summary>
        /// Starts all tasks required for the Client and Server
        /// </summary>
        public virtual void Start()
        {
            Handler.Debug("Starting Background-Process: Instruction-Processing...", DebugType.Info);
            instructionProcessingThread = new Thread(AsyncInstructionProcessingLoop);
            instructionProcessingThread.Start();

            Handler.Debug("Starting Background-Process: Instruction-Sending...", DebugType.Info);
            instructionSendingThread = new Thread(AsyncInstructionSendingLoop);
            instructionSendingThread.Start();
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
                    if (HandlerData.OutgoingInstructions.Count > 0)
                        AsyncInstructionSendNext();
                    else
                        Thread.Sleep(threadIdleTime);
                }
            }
            catch (Exception ex)
            {
                Handler.Debug("Halting (10)", DebugType.Warning);
                if (HandlerData.ShowExceptions) Handler.Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                if (HandlerData.TryRestartOnCrash) HaltAllThreads();
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
                    if (HandlerData.IncommingInstructions.Count > 0)
                        AsyncInstructionProcessNext();
                    else
                        Thread.Sleep(threadIdleTime);
                }
            }
            catch (Exception ex)
            {
                Handler.Debug("Halting (09)", DebugType.Warning);
                if (HandlerData.ShowExceptions) Handler.Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                if (HandlerData.TryRestartOnCrash) HaltAllThreads();
            }
        }


        /// <summary>
        /// Sends the next instruction from the outgoing-queue.
        /// </summary>
        protected virtual void AsyncInstructionSendNext()
        {
            if (!haltActive)
            {
                HandlerData.LogSendCounter++;
                if (HandlerData.OutgoingInstructions[0].GetType() != typeof(InstructionLibraryEssentials.ReceptionConfirmation))
                    HandlerData.LogOutgoingInstructions.Add(HandlerData.OutgoingInstructions[0].Clone());
            }
        }

        /// <summary>
        /// Processes next instruction in the incomming-queue.
        /// </summary>
        protected virtual void AsyncInstructionProcessNext()
        {
            HandlerData.IncommingInstructions[0].Execute();
            HandlerData.IncommingInstructions.RemoveAt(0);
            HandlerData.LogProcessCounter++;
            Handler.Debug($"Processed Instruction ({HandlerData.LogProcessCounter} - Success-Rate: {(float)(1 - ((float)HandlerData.LogOutgoingInstructions.Count / (float)HandlerData.LogProcessCounter)) * 100}%, {HandlerData.LogOutgoingInstructions.Count} Missing confirmation)", DebugType.Info);
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
        

        protected void CheckHaltingState()
        {
            Handler.Debug("Checking halting-state...", DebugType.Cronjob);
            if (haltActive)
            {
                Handler.Debug("Halting active! Restarting system...", DebugType.Cronjob);
                haltActive = false;
                RestartSystem();
            }
            else Handler.Debug("Halting disabled. Continuing regular operation.", DebugType.Cronjob);
        }

        protected void CheckInstructionIntegrity()
        {
            Handler.Debug("Checking instruction-integrity...", DebugType.Cronjob);

            int msgCtr = 0;

            // Remove instruction that failed to be resent 5 times
            for (int i = HandlerData.LogOutgoingInstructions.Count - 1; i >= 0; i--)
                if (HandlerData.LogOutgoingInstructions.GetAttempts(i) > 20)
                    HandlerData.LogOutgoingInstructions.RemoveAt(i);

            // Re-Send every instruction that is still in the outgoing-log
            for (int i = 0; i < HandlerData.LogOutgoingInstructions.Count; i++)
            {
                HandlerData.OutgoingInstructions.Add(HandlerData.LogOutgoingInstructions[i].Clone());
                HandlerData.LogOutgoingInstructions.AddAttempt(i);
                msgCtr++;
            }

            Handler.Debug($"Integrity-Check done. Re-Sent {msgCtr} message(s)", DebugType.Cronjob);
        }

        /// <summary>
        /// Halts all threads and prepares for a restart.
        /// </summary>
        protected virtual void HaltAllThreads()
        {
            if (!haltActive)
            {
                haltActive = true;

                Handler.Debug("A fatal error occured. Attempting to halt all processes...", DebugType.Fatal);

                // Terminate threads
                Handler.Debug("Halting Instruction-Processing...", DebugType.Fatal);
                try
                {
                    instructionProcessingThread.Abort();
                    Handler.Debug("Successfully stopped Instruction-Processing!", DebugType.Fatal);
                }
                catch (Exception ex)
                {
                    Handler.Debug("Could not stop Instruction-Processing!", DebugType.Fatal);
                    if (HandlerData.ShowExceptions) Handler.Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
                }

                Handler.Debug("Halting Instruction-Sending...", DebugType.Fatal);
                try
                {
                    instructionSendingThread.Abort();
                    Handler.Debug("Successfully stopped Instruction-Sending!", DebugType.Fatal);
                }
                catch (Exception ex)
                {
                    Handler.Debug("Could not stop Instruction-Sending!", DebugType.Fatal);
                    if (HandlerData.ShowExceptions) Handler.Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);
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
            for (int i = 0; i < HandlerData.LogOutgoingInstructions.Count; i++)
                if (HandlerData.LogOutgoingInstructions[i].ID == pInstructionID)
                {
                    HandlerData.LogOutgoingInstructions.RemoveAt(i);
                }
        }

        /// <summary>
        /// Restarts the system.
        /// </summary>
        protected virtual void RestartSystem()
        {
            // Re-Define arrays and lists
            buffer = new byte[bufferSize];
            //HandlerData.IncommingInstructions = new List<InstructionBase>();
            //HandlerData.OutgoingInstructions = new List<InstructionBase>();
        }














        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 1a │ F I E L D S   ( P R I V A T E )                        ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝    

        #region ═╣ F I E L D S   ( P R I V A T E ) ╠═ 
        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 1b │ F I E L D S   ( P R O T E C T E D )                    ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝    

        #region ═╣ F I E L D S   ( P R O T E C T E D ) ╠═ 
        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 1c │ D E L E G A T E S                                      ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝    

        #region ═╣ D E L E G A T E S ╠═ 
        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 2a │ P R O P E R T I E S   ( I N T E R N A L )              ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝  

        #region ═╣ P R O P E R T I E S   ( I N T E R N A L ) ╠═ 
        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 2b │ P R O P E R T I E S   ( P U B L I C )                  ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝  

        #region ═╣ P R O P E R T I E S   ( P U B L I C ) ╠═ 
        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 3  │ C O N S T R U C T O R S                                ║
        // ╚════╧════════════════════════════════════════════════════════╝  

        #region ═╣ C O N S T R U C T O R S ╠═ 
        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 4a │ M E T H O D S   ( P R I V A T E )                      ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝  

        #region ═╣ M E T H O D S   ( P R I V A T E ) ╠═ 
        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 4b │ M E T H O D S   ( P R O T E C T E D )                  ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝ 

        #region ═╣ M E T H O D S   ( P R O T E C T E D ) ╠═ 
        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 4c │ M E T H O D S   ( I N T E R N A L )                    ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝ 

        #region ═╣ M E T H O D S   ( I N T E R N A L ) ╠═ 



        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 4d │ M E T H O D S   ( P U B L I C )                        ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝ 

        #region ═╣ M E T H O D S   ( P U B L I C ) ╠═ 
        #endregion
    }
}
