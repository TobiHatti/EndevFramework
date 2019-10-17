using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreClient
{
    class Client
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();


            factory.HostName = "localhost";
        }
    }
}
