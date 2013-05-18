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
    public interface ITaskClient
    {
        bool HasProgram(string programName);
        void ReceiveProgram(Stream message);
        void ExecuteWorkItem(Stream message);
        bool IsAlive();
    }
    
    public class TaskClient : ITaskClient
    {
        // Fields
        private readonly List<ClientTaskProgram> _programs = new List<ClientTaskProgram>();


        public bool HasProgram(string programName)
        {
            var program = GetProgramByName(programName);
            return program != null && program.IsDownloaded();
        }

        public void ReceiveProgram(Stream message)
        {
            var programMessage = DeserializeMessageStream<ProgramMessage>(message);
            var program = new ClientTaskProgram(programMessage.Name);

            SaveProgram(programMessage);
        }

        public void ExecuteWorkItem(Stream message)
        {
            var workItemMessage = DeserializeMessageStream<WorkItemMessage>(message);
            
            var program = GetProgramByName(workItemMessage.ProgramName);
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

        }

        private static T DeserializeMessageStream<T>(Stream message)
        {
            const int READ_BUFFER_SIZE = 81;
            var buffer = new byte[READ_BUFFER_SIZE];

            while (message.Read(buffer, 0, READ_BUFFER_SIZE) > 0)
            {

            }

            return default(T);
        }
    }
}
