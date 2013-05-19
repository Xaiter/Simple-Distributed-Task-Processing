using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedTaskProcessing
{
    public interface ITaskWorker
    {
        object DoWork(WorkItemMessage message);
    }

    public abstract class MockWorker : ITaskWorker
    {
        public object DoWork(WorkItemMessage message)
        {
            Logger.Trace("MockWorker - DoWork");
            Thread.Sleep(12000);
            return null;
        }
    }
}
