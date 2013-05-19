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

namespace DistributedTaskProcessing
{
    /// <summary>
    /// Provides functionality for a client to receive work from a server.    
    /// </summary>
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

    public class TaskClient : ITaskClient
    {
        // Fields
        private readonly List<ClientTaskProgram> _programs = new List<ClientTaskProgram>();


        public TaskClient()
        {
            
        }

        public bool HasProgram(string programName)
        {
            var program = GetProgramByName(programName);
            return program != null && program.IsDownloaded();
        }

        public void ReceiveProgram(Stream message)
        {
            Logger.Trace("TaskClient - Receiving program...");

            var programMessage = DeserializeMessageStream<ProgramMessage>(message);
            var program = new ClientTaskProgram(programMessage.Name);

            SaveProgram(programMessage);
        }

        public void ExecuteWorkItem(Stream message)
        {
            Logger.Trace("TaskClient - Receiving work item...");

            var workItemMessage = DeserializeMessageStream<WorkItemMessage>(message);
            var program = GetProgramByName(workItemMessage.ProgramName);

            Logger.Trace("Executing work item " + workItemMessage.WorkItemId.ToString());

            var executionDomain = AppDomain.CreateDomain(workItemMessage.WorkItemId.ToString());
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

            while (message.Read(buffer, 0, READ_BUFFER_SIZE) > 0)
            {

            }

            return default(T);
        }
    }
}
