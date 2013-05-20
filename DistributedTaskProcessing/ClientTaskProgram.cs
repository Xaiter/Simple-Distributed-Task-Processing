using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace DistributedTaskProcessing
{
    public class ClientTaskProgram
    {
        // Properties
        public string Name { get; private set; }
        public List<string> AssemblyFileNames { get; set; }


        // Constructor
        public ClientTaskProgram(string programName)
        {
            Name = programName;
            AssemblyFileNames = new List<string>();
        }
        
        
        // Methods
        public bool IsDownloaded()
        {
            foreach (string d in AssemblyFileNames)
            {
                if (!File.Exists(d))
                    return false;
            }
            return true;
        }
    }
}
