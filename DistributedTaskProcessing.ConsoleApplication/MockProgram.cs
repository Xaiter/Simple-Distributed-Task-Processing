using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace DistributedTaskProcessing.ConsoleApplication
{
    public class MockProgram : ITaskProgram
    {
        public string Name { get; set; }

        public FileData[] GetProgramFiles()
        {
            return new FileData[] { new FileData("bin/Debug/DistributedTaskProcessing.dll") };
        }

        public WorkItemMessage[] GetWorkItemMessages()
        {
            return new WorkItemMessage[] {
                new WorkItemMessage()
            };
        }
    }
}
