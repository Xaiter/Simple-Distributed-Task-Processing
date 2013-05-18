using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DistributedTaskProcessing
{
    public class TaskClient : ITaskClient
    {
        private List<ClientTaskToolset> Programs;
        public bool HasProgram(ClientTaskToolset program)
        {
            foreach (ClientTaskToolset p in Programs)
            {
                if (p == program)
                    return p.IsDownloaded();
            }
            return false;
        }

        public void ReceiveProgram(Stream message)
        {
            var programMessage = DeserializeMessageStream<ProgramMessage>(message);
            ClientTaskToolset program = new ClientTaskToolset(programMessage.Name);

            foreach (var assembly in programMessage.Assemblies)
                File.WriteAllBytes(assembly.FileName, assembly.Data);

        }

        public void ExecuteWorkItem(Stream message)
        {
            var workItemMessage = DeserializeMessageStream<WorkItemMessage>(message);
        }

        public bool IsAlive()
        {
            return true;
        }

        private void SaveProgram(ProgramMessage message)
        {

        }

        private string[] GetProgramAssemblyPaths(string programName)
        {
            return null; //todo
        }

        private T DeserializeMessageStream<T>(Stream message)
        {
            return default(T); // eventually I will do magic here
        }
    }
}
