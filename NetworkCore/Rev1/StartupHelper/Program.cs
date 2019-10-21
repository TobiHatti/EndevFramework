using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartupHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            Process.Start(@"..\..\..\EndevFrameworkNetworkCoreRev2\bin\Debug\NetCoreServer.exe");
            Process.Start(@"..\..\..\NetCoreClient\bin\Debug\NetCoreClient.exe");
        }
    }
}
