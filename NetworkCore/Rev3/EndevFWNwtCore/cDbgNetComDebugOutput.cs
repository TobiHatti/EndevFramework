using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EndevFrameworkNetworkCore
{
    /// <summary>
    /// =====================================   <para />
    /// FRAMEWORK: EndevFrameworkNetworkCore    <para />
    /// SUB-PACKAGE: Debugging-Tools            <para />
    /// =====================================   <para />
    /// DESCRIPTION:                            <para />
    /// Provides several options for
    /// displaying the Debug-Output.
    /// </summary>
    public class DebugOutput
    {
#pragma warning disable IDE0060 // unused arguments
        /// <summary>
        /// Disables all debug-outputs.
        /// </summary>
        /// <param name="pMessage"></param>
        /// <param name="pParameters"></param>
        public static void Disable(string pMessage, params object[] pParameters) { }

        /// <summary>
        /// Outputs the Debug-Messages to the system-console.
        /// </summary>
        /// <param name="pMessage">Debug-Message</param>
        /// <param name="pParameters">Output-Parameters [Not required for ToConsole-Method]</param>
        public static void ToConsole(string pMessage, params object[] pParameters) => Console.WriteLine(pMessage);

        /// <summary>
        /// Outputs the Debug-Messages to the debug-console. 
        /// </summary>
        /// <param name="pMessage">Debug-Message</param>
        /// <param name="pParameters">Output-Parameters [Not required for ToDebug-Method]</param>
        public static void ToDebug(string pMessage, params object[] pParameters) => Debug.Print(pMessage);

        /// <summary>
        /// Outputs the Debug-Messages to a given WinForms-Textbox. 
        /// </summary>
        /// <param name="pMessage">Debug-Message</param>
        /// <param name="pParameters">Output-Parameters - First parameter: the target textbox-instance</param>
        public static void ToTextbox(string pMessage, params object[] pParameters)
        {
            (pParameters[0] as TextBox).Text += pMessage + "\r\n";
            (pParameters[0] as TextBox).ScrollToCaret();
        }
#pragma warning restore IDE0060 // unused arguments
    }
}
