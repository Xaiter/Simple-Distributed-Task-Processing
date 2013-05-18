using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedTaskProcessing;
using System.Reflection;

namespace DistributedTaskProcessing.ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            TaskClientService.OpenHost();
            TaskServerService.OpenHost();

        }
    }



}
