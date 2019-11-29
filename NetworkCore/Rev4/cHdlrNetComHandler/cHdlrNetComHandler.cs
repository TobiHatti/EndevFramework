using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EndevFrameworkNetworkCore
{
    public abstract class NetComHandler
    {
        protected volatile HandlerData handlerData = new HandlerData();
        protected volatile NetComOperator ncOperator = null;

        protected Thread cronjobThread = null;
        protected Thread operationThread = null;

        public delegate void DebuggingOutput(string pDebugMessage, DebugType pType, params object[] pParameters);
        protected DebuggingOutput DebugCom = null;
        protected object[] debugParams = null;

        public HandlerData HandlerData { get => handlerData; }

        public NetComHandler()
        {
            handlerData.RSAKeys = RSAHandler.GenerateKeyPair();
        }

        public virtual void Start()
        {
            cronjobThread = new Thread(AsyncCronjobLoop);
            cronjobThread.Start();

            operationThread = new Thread(AsyncOperationLoop);
            operationThread.Start();
        }

        protected void AsyncCronjobLoop() { while (true) { AsyncCronjobCycle(); Thread.Sleep(10000); } }

        protected void AsyncOperationLoop() { while(true) { AsyncOperationCycle(); } }

        protected virtual void AsyncCronjobCycle()
        {
            //CheckHaltingState();
            CheckInstructionIntegrity();
        }

        protected virtual void AsyncOperationCycle()
        {
            ncOperator.Start();

            ncOperator.InstructionProcessingThread.Join();
            ncOperator.InstructionSendingThread.Join();
        }

        //protected void CheckHaltingState()
        //{
        //    Debug("Checking halting-state...", DebugType.Cronjob);
        //    if (handlerData.HaltActive)
        //    {
        //        Debug("Halting active! Restarting system...", DebugType.Cronjob);
        //        //handlerData.HaltActive = false;
        //        //RestartSystem();
        //    }
        //    else Debug("Halting disabled. Continuing regular operation.", DebugType.Cronjob);
        //}

        protected void CheckInstructionIntegrity()
        {
            Debug("Checking instruction-integrity...", DebugType.Cronjob);

            int msgCtr = 0;

            // Remove instruction that failed to be resent 5 times
            for (int i = handlerData.LogOutgoingInstructions.Count - 1; i >= 0; i--)
                if (handlerData.LogOutgoingInstructions.GetAttempts(i) > 20)
                    handlerData.LogOutgoingInstructions.RemoveAt(i);

            // Re-Send every instruction that is still in the outgoing-log
            for (int i = 0; i < handlerData.LogOutgoingInstructions.Count; i++)
            {
                handlerData.OutgoingInstructions.Add(handlerData.LogOutgoingInstructions[i].Clone());
                handlerData.LogOutgoingInstructions.AddAttempt(i);
                msgCtr++;
            }

            Debug($"Integrity-Check done. Re-Sent {msgCtr} message(s)", DebugType.Cronjob);
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

        public string ReadOutputStream()
        {
            if (handlerData.OutputStream.Count == 0) return null;

            string retval = handlerData.OutputStream[0];
            handlerData.OutputStream.RemoveAt(0);
            return retval;
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
        // ║ 4b │ M E T H O D S   ( I N T E R N A L )                    ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝ 

        #region ═╣ M E T H O D S   ( I N T E R N A L ) ╠═ 
        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 4c │ M E T H O D S   ( P R O T E C T E D )                  ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝ 

        #region ═╣ M E T H O D S   ( P R O T E C T E D ) ╠═ 
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
