using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DistributedTaskProcessing
{
    /// <summary>
    /// Providers functionality to add reigster workers
    /// and receive work progress updates.
    /// </summary>    
    [ServiceContract]
    public interface ITaskServer
    {
        [OperationContract]
        Guid RegisterClient(string clientName, string endpointUrl);

        [OperationContract]
        void WorkItemComplete(Guid clientId, Guid workItemId, object returnValue);

        [OperationContract]
        void UnregisterClient(Guid clientId);
    }
}
