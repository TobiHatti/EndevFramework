using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndevFWNetCore
{
    public class NetComDebugOutput
    {
        public static void Disable(string pMessage, params object[] pParameters) { }
        public static void ToConsole(string pMessage, params object[] pParameters) => Console.WriteLine(pMessage);
        public static void ToDebug(string pMessage, params object[] pParameters) => Debug.Print(pMessage);
        public static void ToTextbox(string pMessage, params object[] pParameters)
        {
            //(pParameters[0] as TextBox).Text += pMessage + "\r\n";
            //(pParameters[0] as TextBox).ScrollToCaret();
        }
    }
}
