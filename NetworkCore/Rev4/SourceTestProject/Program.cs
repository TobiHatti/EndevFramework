using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SourceTestProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread HandlerThread = new Thread(Operator);
            HandlerThread.Start();

            while (true) Console.WriteLine("IsAlive: " + HandlerThread.IsAlive);
        }


        public static void Operator()
        {
            Thread.Sleep(5000);
            return;
        }
    }
}
