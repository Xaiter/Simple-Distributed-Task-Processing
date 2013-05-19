using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedTaskProcessing
{
    public static class Logger
    {
        public static void Trace(string message)
        {
            Console.WriteLine(message);
        }

        public static void Exception(string message, Exception ex)
        {

        }
    }
}
