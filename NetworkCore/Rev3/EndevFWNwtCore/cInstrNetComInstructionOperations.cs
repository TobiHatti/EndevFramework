using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndevFWNwtCore
{
    public class InstructionOperations
    {
        public static IEnumerable<InstructionBase> Parse(string pInstructionString)
        {
            foreach(string encodedInstruction in pInstructionString.Split(';'))
            {
                if (string.IsNullOrEmpty(encodedInstruction)) continue;

                string encodedInstructionTrimmed = encodedInstruction.TrimStart('{').TrimEnd('}');
                bool rsaEncoded = false;

                if(encodedInstructionTrimmed.StartsWith("RSA:"))
                {
                    rsaEncoded = true;
                    encodedInstructionTrimmed = encodedInstructionTrimmed.Remove(0, 4);
                }
                



            }

            return null;
        }
    }
}
