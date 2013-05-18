using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DistributedTaskProcessing
{
    public class ClientTaskToolset
    {
        public ClientTaskToolset(string programName)
        {
            Name = programName;
            dlls = new List<string>();
        }
        public string Name { get; private set; }
        public List<string> dlls { get; set; }

        public bool IsDownloaded()
        {
            foreach (string d in dlls)
            {
                if (!File.Exists(d))
                    return false;
            }
            return true;
        }
    }
}
