using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndevFrameworkNetworkCore
{
    /// <summary>
    /// =====================================   <para />
    /// FRAMEWORK: EndevFrameworkNetworkCore    <para />
    /// SUB-PACKAGE: Instruction-Objects        <para />
    /// =====================================   <para />
    /// DESCRIPTION:                            <para />
    /// Logs all outgoing instructions, counting 
    /// how many times it tried to re-send an 
    /// instruction.
    /// </summary>
    public class InstructionOutgoingLog
    {
        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 1a │ F I E L D S   ( P R I V A T E )                        ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝    

        #region ═╣ F I E L D S   ( P R I V A T E ) ╠═ 

        private readonly List<InstructionBase> instructions = new List<InstructionBase>();
        private readonly List<int> resendAttempts = new List<int>();

        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 2b │ P R O P E R T I E S   ( P U B L I C )                  ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝  

        #region ═╣ P R O P E R T I E S   ( P U B L I C ) ╠═ 

        public int Count
        {
            get => instructions.Count;
        }

        public InstructionBase this[int pIndex]
        {
            get
            {
                if (instructions.Count > pIndex) return instructions[pIndex];
                else return null;
            }
        }

        public InstructionBase this[string pInstructionID]
        {
            get
            {
                foreach (InstructionBase ib in instructions)
                    if (ib.ID == pInstructionID) return ib;
                return null;
            }
        }

        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 4d │ M E T H O D S   ( P U B L I C )                        ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝ 

        #region ═╣ M E T H O D S   ( P U B L I C ) ╠═ 

        public void Add(InstructionBase pInstruction)
        {
            instructions.Add(pInstruction);
            resendAttempts.Add(0);
        }

        public void RemoveAt(int pIndex)
        {
            instructions.RemoveAt(pIndex);
            resendAttempts.RemoveAt(pIndex);
        }

        public int GetAttempts(int pIndex)
        {
            return resendAttempts[pIndex];
        }

        public void AddAttempt(int pIndex)
        {
            resendAttempts[pIndex]++;
        }

        #endregion
    }
}
