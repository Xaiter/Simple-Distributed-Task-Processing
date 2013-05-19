using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Threading;
using DistributedTaskProcessing;

namespace MockObjects
{
    public class MockProgram : ITaskProgram
    {
        public string Name { get; set; }

        public MockProgram()
        {
            Name = "MockProgram";
        }

        public FileData[] GetProgramFiles()
        {
            return new FileData[] { new FileData("MockObjects.dll"), new FileData("DistributedTaskProcessing.dll") };
        }

        public WorkItemMessage[] GetWorkItemMessages()
        {
            return new WorkItemMessage[] {
                new WorkItemMessage() {
                    WorkItemId = Guid.NewGuid(),
                    ProgramName = "MockProgram",
                    WorkerAssemblyName = "MockObjects",
                    WorkerType = "MockObjects.MockWorker",
                }
            };
        }
    }

    [Serializable]
    public class MockWorker : ITaskWorker
    {
        public object DoWork(WorkItemMessage message)
        {
            Logger.Trace("MockWorker - DoWork");
            Thread.Sleep(12000);
            return null;
        }
    }
}
