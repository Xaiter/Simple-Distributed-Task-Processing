using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DistributedTaskProcessing;

namespace DistributedTaskProcessing.Client
{
    /// <summary>
    /// Provides functionality for a client to receive work from a server.    
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class TaskClient : ITaskClient
    {
        // Fields
        private readonly List<ClientTaskProgram> _programs = new List<ClientTaskProgram>();
        private readonly Func<WorkItemMessage, ClientTaskProgram, object> _asyncHandle = null;
        private bool _isBusy = false;


        // Properties
        public Guid? ClientId
        {
            get; set;
        }


        // Constructors
        public TaskClient()
        {
            _asyncHandle = ExecuteWorkItem;
        }



        // Methods
        public bool HasProgram(string programName)
        {
            var program = GetProgramByName(programName);
            return program != null && program.IsDownloaded();
        }

        public void ReceiveProgram(Stream message)
        {
            Logger.Trace("TaskClient - Receiving program...");
            var programMessage = DeserializeMessageStream<ProgramMessage>(message);
            SaveProgram(programMessage);
        }

        public void ExecuteWorkItem(Stream message)
        {
            Logger.Trace("TaskClient - Receiving work item...");

            var workItemMessage = DeserializeMessageStream<WorkItemMessage>(message);
            var program = GetProgramByName(workItemMessage.ProgramName);

            AsyncCallback asyncCallback = (IAsyncResult result) => {
                _isBusy = false;
                var returnValue = _asyncHandle.EndInvoke(result);
                TaskClientService.WorkItemComplete(this.ClientId.Value, workItemMessage.WorkItemId, returnValue);
            };

            _asyncHandle.BeginInvoke(workItemMessage, program, asyncCallback, null);
        }

        public bool IsAlive()
        {
            return true;
        }



        // Private Methods
        private ClientTaskProgram GetProgramByName(string programName)
        {
            foreach (var p in _programs)
                if (p.Name == programName)
                    return p;

            return null;
        }



        // Static Methods
        private object ExecuteWorkItem(WorkItemMessage message, ClientTaskProgram program)
        {
            _isBusy = true;
            Logger.Trace("Executing work item " + message.WorkItemId.ToString());

            var executionDomain = AppDomain.CreateDomain(message.WorkItemId.ToString());
            var crossDomainWorkerProxy = executionDomain.CreateInstanceAndUnwrap(message.WorkerAssemblyName, message.WorkerType) as ITaskWorker;
            return crossDomainWorkerProxy.DoWork(message);
        }

        private static void SaveProgram(ProgramMessage message)
        {
            var p = new ClientTaskProgram(message.Name);
            p.AssemblyFileNames = (from f in message.ProgramFiles
                                   select f.Filename).ToList();

            if (Directory.Exists(p.Name))
                Directory.Delete(p.Name, true);
            Directory.CreateDirectory(p.Name);

            foreach (var file in message.ProgramFiles)
            {
                string path = Path.Combine(p.Name, file.Filename);
                File.WriteAllBytes(path, file.Data);
            }
        }

        private static T DeserializeMessageStream<T>(Stream message)
        {
            const int READ_BUFFER_SIZE = 8192;
            var buffer = new byte[READ_BUFFER_SIZE];
            var messageDataStream = new MemoryStream();

            int messageSize = 0;
            int readSize = 0;

            while ((readSize = message.Read(buffer, 0, READ_BUFFER_SIZE)) > 0)
            {
                messageDataStream.Write(buffer, 0, readSize);
                messageSize += readSize;
            }
            message.Close();
            message.Dispose();
            message = null;

            return DataUtilities.Deserialize<T>(messageDataStream.ToArray());
        }
    }
}
