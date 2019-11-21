using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EndevFrameworkNetworkCore
{
    public enum DebugType
    {
        Info,
        Warning,
        Error,
        Fatal,
        Remote,
        Exception,
        Cronjob,
        Confirmation,
    }


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
        public static void Disable(string pMessage, DebugType pDebugType, params object[] pParameters) { }

        /// <summary>
        /// Outputs the Debug-Messages to the system-console.
        /// </summary>
        /// <param name="pMessage">Debug-Message</param>
        /// <param name="pParameters">Output-Parameters [Not required for ToConsole-Method]</param>
        public static void ToConsole(string pMessage, DebugType pDebugType, params object[] pParameters)
        {
            Console.Write($"({Thread.CurrentThread.ManagedThreadId.ToString("D3")})");

            switch (pDebugType)
            {
                case DebugType.Info:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.BackgroundColor = ConsoleColor.Black;

                    Console.Write("[ ~INFO ] ");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.WriteLine(pMessage);
                    break;
                case DebugType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.BackgroundColor = ConsoleColor.Black;

                    Console.Write("[WARNING] ");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.WriteLine(pMessage);
                    break;
                case DebugType.Cronjob:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.BackgroundColor = ConsoleColor.Black;

                    Console.Write("[CRONJOB] ");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.WriteLine(pMessage);
                    break;
                case DebugType.Confirmation:
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.BackgroundColor = ConsoleColor.Black;

                    Console.Write("[CONFIRM] ");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.WriteLine(pMessage);
                    break;
                case DebugType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.BackgroundColor = ConsoleColor.Black;

                    Console.Write("[ ERROR ] ");

                    Console.WriteLine(pMessage);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    break;
                case DebugType.Fatal:
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.DarkRed;

                    Console.Write("[ FATAL ] ");

                    Console.WriteLine(pMessage);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    break;
                case DebugType.Remote:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.BackgroundColor = ConsoleColor.Black;

                    Console.Write("[~REMOTE] ");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.WriteLine(pMessage);
                    
                    break;
                case DebugType.Exception:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.BackgroundColor = ConsoleColor.Black;

                    Console.Write("[EXCEPT.] ");

                    Console.WriteLine(pMessage);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;

                    break;
            }
        }

        /// <summary>
        /// Outputs the Debug-Messages to the debug-console. 
        /// </summary>
        /// <param name="pMessage">Debug-Message</param>
        /// <param name="pParameters">Output-Parameters [Not required for ToDebug-Method]</param>
        public static void ToDebug(string pMessage, DebugType pDebugType, params object[] pParameters) => Debug.Print(pMessage);

        /// <summary>
        /// Outputs the Debug-Messages to a given WinForms-Textbox. 
        /// </summary>
        /// <param name="pMessage">Debug-Message</param>
        /// <param name="pParameters">Output-Parameters - First parameter: the target textbox-instance</param>
        public static void ToTextbox(string pMessage, DebugType pDebugType, params object[] pParameters)
        {
            (pParameters[0] as TextBox).Text += pMessage + "\r\n";
            (pParameters[0] as TextBox).ScrollToCaret();
        }
#pragma warning restore IDE0060 // unused arguments
    }
}
