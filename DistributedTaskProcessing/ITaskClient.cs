using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DistributedTaskProcessing
{
    [ServiceContract]
    public interface ITaskClient
    {
        [OperationContract]
        bool HasProgram(string programName);

        [OperationContract]
        void ReceiveProgram(Stream message);

        [OperationContract]
        void ExecuteWorkItem(Stream message);

        [OperationContract]
        bool IsAlive();
    }
}
