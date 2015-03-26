using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;

namespace Dotpay.Application
{
    class Program
    {
        static void Main(string[] args)
        {
            GrainClient.Initialize("ClientConfiguration.xml");
        }
    }
}
